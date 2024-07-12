namespace AthenasAcademy.Certificate.EventBus.Brokers;

public class RabbitMQSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public RabbitMQSetup Setup { get; set; }
}

public class RabbitMQSetup
{
    public string ExchangeBaseName { get; set; }
    public string Type { get; set; }
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
    public bool Internal { get; set; }
}