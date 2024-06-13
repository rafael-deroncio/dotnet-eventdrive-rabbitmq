using AthenasAcademy.Components.EventBus.Events;
using AthenasAcademy.Components.EventBus.Handlers.Interfaces;

namespace AthenasAcademy.Components.EventBus;

/// <summary>
/// Interface that defines the contract for the EventBus system, providing methods for publishing and subscribing to events.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Asynchronously publishes an event to the EventBus.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event to be published.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : BaseEvent;

    /// <summary>
    /// Asynchronously subscribes to a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the handler that will process the event.</typeparam>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <param name="maxCallbacks">The maximum number of callbacks to be invoked. Defaults to 10.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken cancellationToken, int maxCallbacks = 10)
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;


    /// <summary>
    /// Asynchronously unsubscribes from a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the handler that processes the event.</typeparam>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    /// Publishes an event to the EventBus.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <param name="event">The event to be published.</param>
    void Publish<TEvent>(TEvent @event)
        where TEvent : BaseEvent;

    /// <summary>
    /// Subscribes to a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the handler that will process the event.</typeparam>
    /// <param name="cancellationToken">A cancellation token to observe while the subscription is active.</param>
    /// <param name="maxCallbacks">The maximum number of callbacks to be invoked. Defaults to 10.</param>
    void Subscribe<TEvent, TEventHandler>(CancellationToken cancellationToken, int maxCallbacks = 10)
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;


    /// <summary>
    /// Unsubscribes from a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the handler that processes the event.</typeparam>
    void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;
}
