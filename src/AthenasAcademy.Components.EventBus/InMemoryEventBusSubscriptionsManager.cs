namespace AthenasAcademy.Components.EventBus;

public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;

    public InMemoryEventBusSubscriptionsManager()
    {
        _handlers = [];
        _eventTypes = [];
    }

    public bool IsEmpty => !_handlers.Keys.Any();

    public event EventHandler<string> OnEventRemoved;

    public void AddSubscription<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
    {

        DoAddSubscription(typeof(TEventHandler), GetEventKey<TEvent>());

        if (!_eventTypes.Contains(typeof(TEvent)))
            _eventTypes.Add(typeof(TEvent));
    }

    public void RemoveSubscription<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
        => DoRemoveHandler(
            GetEventKey<TEvent>(),
            FindSubscriptionToRemove<TEvent, TEventHandler>());

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>() where TEvent : BaseEvent => GetHandlersForEvent(GetEventKey<TEvent>());

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    public bool HasSubscriptionsForEvent<TEvent>() where TEvent : BaseEvent => HasSubscriptionsForEvent(GetEventKey<TEvent>());

    public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(@event => @event.Name == eventName);

    public void Clear() => _handlers.Clear();

    public string GetEventKey<T>() => typeof(T).Name;

    #region privates
    private void RaiseOnEventRemoved(string eventName)
    {
        EventHandler<string> handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }

    private void DoRemoveHandler(string eventName, SubscriptionInfo subsToRemove)
    {
        if (subsToRemove != null)
        {
            _handlers[eventName].Remove(subsToRemove);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);

                Type eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);

                if (eventType != null)
                    _eventTypes.Remove(eventType);

                RaiseOnEventRemoved(eventName);
            }
        }
    }

    private SubscriptionInfo FindSubscriptionToRemove<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>
        => DoFindSubscriptionToRemove(
            GetEventKey<TEvent>(),
            typeof(TEventHandler)
        );

    private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType) 
        => !_handlers.ContainsKey(eventName) ? null : _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);

    private void DoAddSubscription(Type handlerType, string eventName)
    {
        if (!_handlers.ContainsKey(eventName))
            _handlers.Add(eventName, new List<SubscriptionInfo>());


        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            throw new ArgumentException(
                $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }
    #endregion
}
