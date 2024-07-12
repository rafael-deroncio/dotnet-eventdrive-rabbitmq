namespace AthenasAcademy.Certificate.EventBus;

/// <summary>
/// Interface that defines the contract for managing event subscriptions in the EventBus system.
/// </summary>
public interface IEventBusSubscriptionsManager
{
    /// <summary>
    /// Gets a value indicating whether there are no subscriptions.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Event that is triggered when a subscription is removed.
    /// </summary>
    event EventHandler<string> OnEventRemoved;

    /// <summary>
    /// Adds a subscription for a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the handler that will process the event.</typeparam>
    void AddSubscription<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    /// Removes a subscription for a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <typeparam name="TEventHandler">The type of the handler that will process the event.</typeparam>
    void RemoveSubscription<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    /// <summary>
    /// Checks if there are subscriptions for a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <returns>True if there are subscriptions; otherwise, false.</returns>
    bool HasSubscriptionsForEvent<TEvent>() where TEvent : BaseEvent;

    /// <summary>
    /// Checks if there are subscriptions for a specific event by event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>True if there are subscriptions; otherwise, false.</returns>
    bool HasSubscriptionsForEvent(string eventName);

    /// <summary>
    /// Gets the event type by event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>The event type.</returns>
    Type GetEventTypeByName(string eventName);

    /// <summary>
    /// Clears all subscriptions.
    /// </summary>
    void Clear();

    /// <summary>
    /// Gets the handlers for a specific event.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <returns>A collection of subscription information.</returns>
    IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>() where TEvent : BaseEvent;

    /// <summary>
    /// Gets the handlers for a specific event by event name.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>A collection of subscription information.</returns>
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

    /// <summary>
    /// Gets the event key for a specific event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <returns>The event key.</returns>
    string GetEventKey<TEvent>();
}
