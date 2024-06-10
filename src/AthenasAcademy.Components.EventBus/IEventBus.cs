using AthenasAcademy.Components.EventBus.Events;
using AthenasAcademy.Components.EventBus.Handlers.Interfaces;

namespace AthenasAcademy.Components.EventBus;

public interface IEventBus
{
    void Publish(BaseEvent @event);

    Task PublishAsync(BaseEvent @event);

    void Subscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    Task SubscribeAsync<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    void Unsubscribe<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;

    Task UnsubscribeAsync<TEvent, TEventHandler>()
        where TEvent : BaseEvent
        where TEventHandler : IEventHandler<TEvent>;
}
