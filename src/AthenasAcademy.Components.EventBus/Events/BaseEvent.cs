using System.Text.Json.Serialization;

namespace AthenasAcademy.Components.EventBus.Events;

public class BaseEvent : IDisposable
{
    [JsonIgnore]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [JsonIgnore]
    public DateTime CreationDate { get; private set; } = DateTime.Now;

    [JsonIgnore]
    public int RetryCount { get; set; } = 0;

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if(!_disposed && disposing) 
            _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
