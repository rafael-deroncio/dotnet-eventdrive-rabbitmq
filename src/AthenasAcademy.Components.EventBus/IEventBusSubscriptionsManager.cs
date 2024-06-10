using AthenasAcademy.Components.EventBus.Events;
using AthenasAcademy.Components.EventBus.Handlers.Interfaces;

namespace AthenasAcademy.Components.EventBus;

public interface IEventBusSubscriptionsManager
{
    bool IsEmpty { get; }
    event EventHandler<string> OnEventRemoved;

    void AddSubscription<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    void RemoveSubscription<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    bool HasSubscriptionsForEvent<TEvent>() where TEvent : BaseEvent;
    bool HasSubscriptionsForEvent(string eventName);
    Type GetEventTypeByName(string eventName);
    void Clear();
    IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>() where TEvent : BaseEvent;
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<TEvent>();
}

