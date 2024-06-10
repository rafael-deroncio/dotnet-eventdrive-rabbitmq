using RabbitMQ.Client;

namespace AthenasAcademy.Components.EventBus.Brokers.Interfaces;

public interface IRabbitMQPersistentConnection
{
    bool IsConnected { get; }
    bool TryConnect();
    IModel CreateModel();
    void Dispose();
}