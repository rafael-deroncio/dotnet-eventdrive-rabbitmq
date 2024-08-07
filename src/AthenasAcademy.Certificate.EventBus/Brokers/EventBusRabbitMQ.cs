﻿using System.Text;
using System.Text.Json;
using Autofac;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AthenasAcademy.Certificate.EventBus.Brokers.Interfaces;
using AthenasAcademy.Certificate.EventBus.Brokers.Constants;

namespace AthenasAcademy.Certificate.EventBus.Brokers;

public class EventBusRabbitMQ(
    ILogger<EventBusRabbitMQ> logger,
    IRabbitMQPersistentConnection persistentConnection,
    IEventBusSubscriptionsManager subscriptionsManager,
    ILifetimeScope lifetime,
    string exchange)
: IEventBus, IDisposable
{
    private readonly ILogger<EventBusRabbitMQ> _logger = logger;
    private readonly IRabbitMQPersistentConnection _persistentConnection = persistentConnection;
    private readonly IEventBusSubscriptionsManager _subscriptionsManager = subscriptionsManager;
    private readonly ILifetimeScope _lifetime = lifetime;
    private readonly string _exchange = exchange.Trim().ToLower();

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : BaseEvent
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        using (var channel = _persistentConnection.CreateModel())
        {
            string exchange = GetExchangeName<TEvent>();
            string queue = GetQueueName<TEvent>();
            string routingKey = GetRoutingKeyName<TEvent>();

            Dictionary<string, object> arguments = await DeclareDeadLetterQueue<TEvent>(channel);

            channel.ExchangeDeclare(exchange, RabbitMQConstants.EXCHANGE_TYPE);
            channel.QueueDeclare(queue, true, false, false, arguments);
            channel.QueueBind(queue, exchange, routingKey);

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange, routingKey, properties, SerializeEvent(@event));
        }
    }

    public async Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken, int maxAttemps = 10, int maxCallbacks = 10)
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {
        if (!_persistentConnection.IsConnected)
            _persistentConnection.TryConnect();

        using (IModel channel = _persistentConnection.CreateModel())
        {
            string exchange = GetExchangeName<TEvent>();
            string queue = GetQueueName<TEvent>();
            string routingKey = GetRoutingKeyName<TEvent>();

            Dictionary<string, object> arguments = await DeclareDeadLetterQueue<TEvent>(channel);

            channel.ExchangeDeclare(exchange, RabbitMQConstants.EXCHANGE_TYPE);
            channel.QueueDeclare(queue, true, false, false, arguments);
            channel.QueueBind(queue, exchange, routingKey);

            if (!_subscriptionsManager.HasSubscriptionsForEvent<TEvent>())
                _subscriptionsManager.AddSubscription<TEvent, TEventHandler>();

            AsyncEventingBasicConsumer consumer = new(channel);

            SemaphoreSlim semaphore = new(initialCount: maxCallbacks > 10 ? 10 : maxCallbacks);
            consumer.Received += async (sender, args) =>
            {
                await semaphore.WaitAsync(cancellationToken);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await OnConsumerReceived<TEvent>(args, channel, queue, maxAttemps);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken);
            };

            channel.BasicConsume(queue, autoAck: false, consumer);

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
            _logger.LogDebug("Starting consumer unsubscription to queue {0}.", queue);

            _subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();
            channel.QueueDelete(queue: queue);

            _logger.LogDebug("Finishing unsubscribing from consumer in queue {Queue} for EventHandler {Handler}.", queue, handler);
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

    public void Subscribe<TEvent, TEventHandler>(CancellationToken cancellation, int maxAttemps = 10, int maxCallbacks = 10)
    where TEvent : BaseEvent where TEventHandler : IEventHandler<TEvent>
        => SubscribeAsync<TEvent, TEventHandler>(cancellation, maxAttemps, maxCallbacks).Wait(cancellation);

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

    private async Task OnConsumerReceived<TEvent>(BasicDeliverEventArgs args, IModel channel, string queue, int maxAttemps) where TEvent : BaseEvent
    {
        string message = Encoding.UTF8.GetString(args.Body.ToArray());
        TEvent @event = DeserializeEvent<TEvent>(message);

        try
        {
            _logger.LogDebug("Starting consumption of the {0} for message {1}.", queue, @event.Id);

            await ProcessEvent(@event).WaitAsync(CancellationToken.None);
            channel.BasicAck(args.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message in queue {Queue}. Attemp {Attemp}", queue, ++@event.RetryCount);

            if (@event.RetryCount < maxAttemps) await PublishAsync<TEvent>(@event);
            else await SendToDeadLetterQueue(channel, @event, args.BasicProperties);

            channel.BasicNack(args.DeliveryTag, multiple: false, false);
        }
        finally
        {
            _logger.LogDebug("Finishing consumption of queue {0} for message {1}.", queue, @event.Id);
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

        _logger.LogDebug("Message {0} send to dead latter queue.", @event.Id);
        await Task.CompletedTask;
    }

    private async Task ProcessEvent<TEvent>(TEvent @event)
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
            _logger.LogDebug("{0} already added", GetEventName<TEvent>());
        }
    }

    private static byte[] SerializeEvent<TEvent>(TEvent @event)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

    private static TEvent DeserializeEvent<TEvent>(string body)
        => JsonSerializer.Deserialize<TEvent>(body);

    private static string GetQueueName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_QUEUE, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").ToLower());

    private string GetExchangeName<TEvent>()
        => string.Format("{0}_{1}_{2}", RabbitMQConstants.PREFIX_EXCHANGE, _exchange, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").ToLower());

    private static string GetDQLQueueName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_DLQ_QUEUE, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").ToLower());

    private string GetDQLExchangeName<TEvent>()
        => string.Format("{0}_{1}_{2}", RabbitMQConstants.PREFIX_DLQ_EXCHANGE, _exchange, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").ToLower());

    private static string GetRoutingKeyName<TEvent>()
        => string.Format("{0}_{1}", RabbitMQConstants.PREFIX_ROUTING_KEY, typeof(TEvent).Name.Replace(RabbitMQConstants.SUFIX_EVENT, "").ToLower());

    private static string GetEventName<TEvent>()
        => typeof(TEvent).Name;

    private static string GetEventHandlerName<TEventHandler>()
        => typeof(TEventHandler).Name;
    #endregion
}
