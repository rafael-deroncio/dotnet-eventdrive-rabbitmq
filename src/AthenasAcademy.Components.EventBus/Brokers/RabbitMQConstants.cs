using RabbitMQ.Client;

namespace AthenasAcademy.Components.EventBus.Brokers.Constants;

public record RabbitMQConstants
{
    public const string AUTOFAC_SCOPE_NAME = "event_bus_rabbitmq";
    public const string SUFIX_EVENT = "Event";
    public const string SUFIX_EVENT_HANDLER = "EventHandler";
    public const string EXCHANGE_TYPE = ExchangeType.Direct;
    public const string PREFIX_EXCHANGE = "exchange";
    public const string PREFIX_QUEUE = "queue";
    public const string PREFIX_ROUTING_KEY = "key_event";
    public const string PREFIX_DLQ_EXCHANGE = "dql_exchange";
    public const string PREFIX_DLQ_QUEUE = "dql_queue";
    public const string X_DLQ_EXCHANGE = "x-dead-letter-exchange";
}
