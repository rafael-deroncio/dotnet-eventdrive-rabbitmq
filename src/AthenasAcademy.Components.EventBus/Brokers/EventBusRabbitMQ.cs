using System.Text;
using System.Text.Json;
using Autofac;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AthenasAcademy.Components.EventBus.Brokers.Interfaces;
using AthenasAcademy.Components.EventBus.Brokers.Constants;

namespace AthenasAcademy.Components.EventBus.Brokers;

public class EventBusRabbitMQ(
    ILogger<EventBusRabbitMQ> logger,
    IRabbitMQPersistentConnection persistentConnection,
    IEventBusSubscriptionsManager subscriptionsManager,
    ILifetimeScope lifetime)
: IEventBus, IDisposable
{
    private readonly ILogger<EventBusRabbitMQ> _logger = logger;
    private readonly IRabbitMQPersistentConnection _persistentConnection = persistentConnection;
    private readonly IEventBusSubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly ILifetimeScope _lifetime = lifetime;

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        try
        {
            if (!_persistentConnection.IsConnected)
                _persistentConnection.TryConnect();

            using (var channel = _persistentConnection.CreateModel())
            {
                string exchange = GetExchangeName<TEvent>();
                string type = RabbitMQConstants.EXCHANGE_TYPE;
                string queue = GetQueueName<TEvent>();
                string routingKey = GetRoutingKeyName<TEvent>();

                Dictionary<string, object> arguments = await DeclareDeadLetterQueue<TEvent>(channel);

                channel.ExchangeDeclare(exchange, type);
                channel.QueueDeclare(queue, true, false, false, arguments);
                channel.QueueBind(queue, exchange, routingKey);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                if (@event.RetryCount > 0)
                    _logger.LogInformation("Starting publishing the message to queue {0}.", queue);

                channel.BasicPublish(exchange, routingKey, properties, SerializeEvent(@event));

                if (@event.RetryCount > 0)
                    _logger.LogInformation("Finishing publishing the message to queue {0}.", queue);

                await Task.CompletedTask;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on publish message.");
            _persistentConnection.Dispose();
            throw;
        }
    }

    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken, int maxCallbacks = 10)
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        using (IModel channel = _persistentConnection.CreateModel())
        {
            string exchange = GetExchangeName<TEvent>();
            string queue = GetQueueName<TEvent>();

            Dictionary<string, object> arguments = await DeclareDeadLetterQueue<TEvent>(channel);

            channel.ExchangeDeclare(exchange, RabbitMQConstants.EXCHANGE_TYPE);
            channel.QueueDeclare(queue, true, false, false, arguments);
            channel.QueueBind(queue, exchange, GetRoutingKeyName<TEvent>());

            AsyncEventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(queue, autoAck: false, consumer);

            if (!_subscriptionsManager.HasSubscriptionsForEvent<TEvent>())
                _subscriptionsManager.AddSubscription<TEvent, TEventHandler>();

            consumer.Received += async (sender, args) => await OnConsumerReceived<TEvent>(sender, args, channel, queue, maxCallbacks);


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
    }

    public async Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        _subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();

        string queue = GetQueueName<TEvent>();
        string handler = GetEventHandlerName<TEventHandler>();

        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        using (IModel channel = _persistentConnection.CreateModel())
        {
            _logger.LogInformation("Starting consumer unsubscription to queue {0}.", queue);

            _subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();
            channel.QueueDelete(queue: queue);

            _logger.LogInformation("Finishing unsubscribing from consumer in queue {0} for EventHandler {1}.", queue, handler);
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _subscriptionsManager.Clear();
        _persistentConnection.Dispose();
    }

    #region syncs
    public void Publish<TEvent>(TEvent @event) where TEvent : BaseEvent
        => PublishAsync(@event).Wait();

    public void Subscribe<TEvent, TEventHandler>(CancellationToken cancellation, int maxCallbacks = 10)
    where TEvent : BaseEvent where TEventHandler : IEventHandler<TEvent>
        => SubscribeAsync<TEvent, TEventHandler>(cancellation, maxCallbacks).Wait(cancellation);

    public void Unsubscribe<TEvent, TEventHandler>()
    where TEvent : BaseEvent where TEventHandler : IEventHandler<TEvent>
        => UnsubscribeAsync<TEvent, TEventHandler>().Wait();

    #endregion

    #region privates
    private async Task<Dictionary<string, object>> DeclareDeadLetterQueue<TEvent>(IModel channel)
    {
        string exchangeDql = GetDQLExchangeName<TEvent>();
        string queueDql = GetDQLQueueName<TEvent>();

        channel.ExchangeDeclare(exchangeDql, RabbitMQConstants.EXCHANGE_TYPE);
        channel.QueueDeclare(queueDql, true, false, false, null);
        channel.QueueBind(queueDql, exchangeDql, string.Empty);

        await Task.CompletedTask;
        return new() { { RabbitMQConstants.X_DLQ_EXCHANGE, exchangeDql } };
    }

    private async Task OnConsumerReceived<TEvent>(object sender, BasicDeliverEventArgs args, IModel channel, string queue, int maxCallbacks) where TEvent : BaseEvent
    {
        string message = Encoding.UTF8.GetString(args.Body.ToArray());
        TEvent @event = DeserializeEvent<TEvent>(message);

        try
        {
            _logger.LogInformation("Starting consumption of the {0} queue.", queue);

            await ProccessEvent(@event);
            channel.BasicAck(args.DeliveryTag, false);

            _logger.LogInformation("Finishing consumption of queue {0} for message {1}.", queue, @event.Id);
        }
        catch (Exception ex)
        {
            @event.RetryCount++;

            if (@event.RetryCount <= maxCallbacks)
            {
                _logger.LogError(ex, "Error processing message in queue {0}. Retrying message.", queue);
                await PublishAsync<TEvent>(@event);
            }
            else
            {
                _logger.LogWarning("Max retry count reached, sending to dead letter queue.");
                await SendToDeadLetterQueue(channel, @event, args.BasicProperties);
            }

            channel.BasicNack(args.DeliveryTag, multiple: false, false);
        }
    }

    private async Task SendToDeadLetterQueue<TEvent>(IModel channel, TEvent @event, IBasicProperties properties) where TEvent : BaseEvent
    {
        string exchangeDql = GetDQLExchangeName<TEvent>();
        string queueDql = GetDQLQueueName<TEvent>();

        channel.ExchangeDeclare(exchangeDql, RabbitMQConstants.EXCHANGE_TYPE);
        channel.QueueDeclare(queueDql, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queueDql, exchangeDql, string.Empty);
        channel.BasicPublish(exchangeDql, string.Empty, properties, SerializeEvent(@event));

        await Task.CompletedTask;
    }

    private async Task ProccessEvent<TEvent>(TEvent @event)
    {
        string eventName = _subscriptionsManager.GetEventKey<TEvent>();

        if (_subscriptionsManager.HasSubscriptionsForEvent(eventName))
        {
            using (ILifetimeScope scope = _lifetime.BeginLifetimeScope(RabbitMQConstants.AUTOFAC_SCOPE_NAME))
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

    private static byte[] SerializeEvent<TEvent>(TEvent @event)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

    private static TEvent DeserializeEvent<TEvent>(string body)
        => JsonSerializer.Deserialize<TEvent>(body);

    private static string GetQueueName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_QUEUE, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").Replace(RabbitMQConstants.SUFIX_EVENT_HANDLER, "").ToLower());

    private string GetExchangeName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_EXCHANGE, typeof(TEvent).Name.Trim().Replace(RabbitMQConstants.SUFIX_EVENT, "").Replace(RabbitMQConstants.SUFIX_EVENT_HANDLER, "").ToLower());

    private static string GetDQLQueueName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_DLQ_QUEUE, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").Replace(RabbitMQConstants.SUFIX_EVENT_HANDLER, "").ToLower());

    private string GetDQLExchangeName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_DLQ_EXCHANGE, typeof(TEvent).Name.Trim().Replace(RabbitMQConstants.SUFIX_EVENT, "").Replace(RabbitMQConstants.SUFIX_EVENT_HANDLER, "").ToLower());

    private static string GetRoutingKeyName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_ROUTING_KEY, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").Replace(RabbitMQConstants.SUFIX_EVENT_HANDLER, "").ToLower());

    private static string GetEventName<TEvent>()
        => typeof(TEvent).Name;

    private static string GetEventHandlerName<TEventHandler>()
        => typeof(TEventHandler).Name;
    #endregion
}
