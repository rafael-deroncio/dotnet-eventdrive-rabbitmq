using System.Text;
using System.Text.Json;
using AthenasAcademy.Components.EventBus.Brokers.Interfaces;
using AthenasAcademy.Components.EventBus.Events;
using AthenasAcademy.Components.EventBus.Handlers.Interfaces;
using Autofac;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AthenasAcademy.Components.EventBus.Brokers;

public class EventBusRabbitMQ(
    ILogger<EventBusRabbitMQ> logger,
    IRabbitMQPersistentConnection persistentConnection,
    IEventBusSubscriptionsManager subscriptionsManager,
    ILifetimeScope lifetime,
    string exchangeBaseName)
: IEventBus, IDisposable
{
    private readonly ILogger<EventBusRabbitMQ> _logger = logger;
    private readonly IRabbitMQPersistentConnection _persistentConnection = persistentConnection;
    private readonly IEventBusSubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly ILifetimeScope _lifetime = lifetime;
    private readonly string _exchangeBaseName = exchangeBaseName;

    private const string AUTOFAC_SCOPE_NAME = "event_bus_rabbitmq";
    private const string SUFIX_EVENT = "Event";
    private const string SUFIX_EVENT_HANDLER = "EventHandler";
    private const string EXCHANGE_TYPE = ExchangeType.Direct;
    private const string PREFIX_EXCHANGE = "exchange";
    private const string PREFIX_QUEUE = "queue";
    private const string PREFIX_ROUTING_KEY = "key_event";
    private const string PREFIX_DLQ_EXCHANGE = "dql_exchange";
    private const string PREFIX_DLQ_QUEUE = "dql_queue";

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        string exchange = GetExchangeName();
        string type = EXCHANGE_TYPE;
        string queue = GetQueueName<TEvent>();
        string routingKey = GetRoutingKeyName<TEvent>();

        using (var channel = _persistentConnection.CreateModel())
        {
            channel.ExchangeDeclare(exchange, type);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey);

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            _logger.LogInformation("Starting publishing the message to queue {0}.", queue);

            await Task.Run(() =>
                channel.BasicPublish(exchange, routingKey, properties, SerializeEvent(@event))
            );

            _logger.LogInformation("Finishing publishing the message to queue {0}.", queue);
        }
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : BaseEvent => PublishAsync(@event).Wait();

    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken)
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        IModel channel = _persistentConnection.CreateModel();

        await DeclareDeadLetterQueue<TEvent>(channel);

        string queue = GetQueueName<TEvent>();
        string exchange = GetExchangeName();
        string routingKey = GetRoutingKeyName<TEvent>();

        uint prefetchSize = 0;
        ushort prefetchCount = 10;
        bool global = false;

        channel.QueueDeclare(queue, true, false, false, null);
        channel.QueueBind(queue, exchange, routingKey);
        channel.BasicQos(prefetchSize, prefetchCount, global);

        AsyncEventingBasicConsumer consumer = new(channel);

        channel.BasicConsume(queue, autoAck: false, consumer);

        if (!_subscriptionsManager.HasSubscriptionsForEvent<TEvent>())
            _subscriptionsManager.AddSubscription<TEvent, TEventHandler>();

        consumer.Received += async (sender, args) => await OnConsumerReceived<TEvent>(sender, args, channel, queue);

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            channel.Close();
            channel.Dispose();
        }
    }

    public void Subscribe<TEvent, TEventHandler>(CancellationToken cancellation)
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
        => SubscribeAsync<TEvent, TEventHandler>(cancellation).Wait(cancellation);

    public Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        _subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();

        string queue = GetQueueName<TEvent>();
        string handler = GetEventHandlerName<TEventHandler>();

        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        using (RabbitMQ.Client.IModel channel = _persistentConnection.CreateModel())
        {
            _logger.LogInformation("Starting consumer unsubscription to queue {0}.", queue);
            _subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();
            channel.QueueDelete(queue: queue);
            _logger.LogInformation("Finishing unsubscribing from consumer in queue {0} for EventHandler {1}.", queue, handler);

        }

        return Task.CompletedTask;
    }

    public void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        UnsubscribeAsync<TEvent, TEventHandler>();
    }

    public void Dispose() => _subscriptionsManager.Clear();

    #region privates
    private async Task<Dictionary<string, object>> DeclareDeadLetterQueue<TEvent>(IModel channel)
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        string exchange = GetDQLExchangeName();
        string type = EXCHANGE_TYPE;
        string queue = GetDQLQueueName<TEvent>();
        string routingKey = string.Empty;

        channel.ExchangeDeclare(exchange, type);
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue, exchange, routingKey);

        return await Task.FromResult(
            new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", exchange }
            }
        );
    }
    private async Task OnConsumerReceived<TEvent>(object sender, BasicDeliverEventArgs args, IModel channel, string queue)
    {
        string message = Encoding.UTF8.GetString(args.Body.ToArray());
        TEvent @event = DeserializeEvent<TEvent>(message);

        try
        {
            _logger.LogInformation("Starting consumption of the {0} queue.", queue);

            await ProccessEvent(@event);
            channel.BasicAck(args.DeliveryTag, false); // commit

            _logger.LogInformation("Finishing consumption of queue {0} for message {1}.", queue, (@event as BaseEvent).Id);
        }
        catch (Exception ex)
        {
            if ((@event as BaseEvent).RetryCount == 10)
            {
                
            }
            else
            {
                _logger.LogError(ex, "Error processing message in queue {0}.", queue);
                channel.BasicNack(args.DeliveryTag, false, true); // rollback
            }
        }
    }

    private async Task ProccessEvent<TEvent>(TEvent @event)
    {
        string eventName = _subscriptionsManager.GetEventKey<TEvent>();

        if (_subscriptionsManager.HasSubscriptionsForEvent(eventName))
        {
            using (ILifetimeScope scope = _lifetime.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
            {
                IEnumerable<SubscriptionInfo> subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);

                foreach (SubscriptionInfo subscription in subscriptions)
                {
                    Type eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                    Type concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                    object handler = scope.ResolveOptional(subscription.HandlerType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, [@event]);
                }
            }
        }
        else
        {
            _logger.LogInformation("{0} already added", GetEventName<TEvent>());
        }
    }

    private static byte[] SerializeEvent<TEvent>(TEvent @event) => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

    private static TEvent DeserializeEvent<TEvent>(string body) => JsonSerializer.Deserialize<TEvent>(body);

    private static string GetQueueName<TEvent>() => string.Format("{0}_{1}", PREFIX_QUEUE, typeof(TEvent).Name.Replace(SUFIX_EVENT, "").Replace(SUFIX_EVENT_HANDLER, "").ToLower());

    private string GetExchangeName() => string.Format("{0}_{1}", PREFIX_EXCHANGE, _exchangeBaseName.Trim().Replace(SUFIX_EVENT, "").Replace(SUFIX_EVENT_HANDLER, "").ToLower());

    private static string GetDQLQueueName<TEvent>() => string.Format("{0}_{1}", PREFIX_DLQ_QUEUE, typeof(TEvent).Name.Replace(SUFIX_EVENT, "").Replace(SUFIX_EVENT_HANDLER, "").ToLower());

    private string GetDQLExchangeName() => string.Format("{0}_{1}", PREFIX_DLQ_EXCHANGE, _exchangeBaseName.Trim().Replace(SUFIX_EVENT, "").Replace(SUFIX_EVENT_HANDLER, "").ToLower());

    private static string GetRoutingKeyName<TEvent>() => string.Format("{0}_{1}", PREFIX_ROUTING_KEY, typeof(TEvent).Name.Replace(SUFIX_EVENT, "").Replace(SUFIX_EVENT_HANDLER, "").ToLower());

    private static string GetEventName<TEvent>() => typeof(TEvent).Name;

    private static string GetEventHandlerName<TEventHandler>() => typeof(TEventHandler).Name;
    #endregion
}
