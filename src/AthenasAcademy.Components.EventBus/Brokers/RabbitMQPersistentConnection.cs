using AthenasAcademy.Components.EventBus.Brokers.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace AthenasAcademy.Components.EventBus.Brokers;

public class RabbitMQPersistentConnection(IConnectionFactory connectionFactory) : IRabbitMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory = connectionFactory
            ?? throw new ArgumentNullException(nameof(connectionFactory));
            
    private IConnection _connection;
    private bool _disposed;

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    public IModel CreateModel()
    {
        if (!IsConnected)
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");

        return _connection.CreateModel();
    }

    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;

        try { _connection.Dispose(); }
        catch (IOException) { }
    }

    public bool TryConnect()
    {
        lock (this)
        {
            try
            {
                if (IsConnected)
                    return true;
                _connection = _connectionFactory.CreateConnection();
            }
            catch (BrokerUnreachableException)
            {
                return false;
            }

            if (IsConnected)
                return true;
            
            return false;
        }
    }
}
