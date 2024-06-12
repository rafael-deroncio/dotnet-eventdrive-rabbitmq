using AthenasAcademy.Components.EventBus.Events;

namespace AthenasAcademy.Components.EventBus.Handlers.Interfaces;

/// <summary>
/// Interface that defines the contract for handling events.
/// </summary>
/// <typeparam name="TEvent">The type of event to handle.</typeparam>
public interface IEventHandler<in TEvent> : IEventHandler where TEvent : BaseEvent
{
    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Handle(TEvent @event);
}

/// <summary>
/// Marker interface for event handlers.
/// </summary>
public interface IEventHandler { }
