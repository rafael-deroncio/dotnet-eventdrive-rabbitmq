using AthenasAcademy.Components.EventBus.Events;

namespace AthenasAcademy.Components.EventBus.Handlers.Interfaces;

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : BaseEvent
{
    Task Handle(TEvent @event);
}

public interface IEventHandler { }

