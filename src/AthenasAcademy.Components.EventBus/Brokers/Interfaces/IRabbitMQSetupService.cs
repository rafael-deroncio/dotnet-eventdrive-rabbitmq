namespace AthenasAcademy.Components.EventBus.Brokers.Interfaces;

public interface IRabbitMQSetupService
{
    Task StartExchangeAsync();
}