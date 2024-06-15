namespace AthenasAcademy.Components.EventBus;

/// <summary>
/// Represents the base class for all events in the EventBus system.
/// </summary>
public class BaseEvent : IDisposable
{
    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Gets the creation date of the event.
    /// </summary>
    public DateTime CreationDate { get; private set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the retry count for the event.
    /// </summary>
    public int RetryCount { get; set; } = 0;

    private bool _disposed;

    /// <summary>
    /// Releases the unmanaged resources used by the BaseEvent and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _disposed = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
