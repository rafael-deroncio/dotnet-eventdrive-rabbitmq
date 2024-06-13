using RabbitMQ.Client;

namespace AthenasAcademy.Components.EventBus.Brokers.Interfaces;

/// <summary>
/// Interface for managing a persistent connection to RabbitMQ.
/// </summary>
public interface IRabbitMQPersistentConnection
{
    /// <summary>
    /// Gets a value indicating whether the connection to RabbitMQ is currently active.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Attempts to establish a connection to RabbitMQ.
    /// </summary>
    /// <returns>true if the connection was successfully established; otherwise, false.</returns>
    bool TryConnect();

    /// <summary>
    /// Creates a new channel (IModel) for communication with RabbitMQ.
    /// </summary>
    /// <returns>An instance of IModel representing the created channel.</returns>
    IModel CreateModel();

    /// <summary>
    /// Releases all resources allocated by the persistent connection.
    /// </summary>
    void Dispose();
}
