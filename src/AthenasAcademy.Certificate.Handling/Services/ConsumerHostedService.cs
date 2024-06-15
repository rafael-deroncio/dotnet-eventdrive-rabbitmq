using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Handling.Handlers;
using AthenasAcademy.Components.EventBus;

namespace AthenasAcademy.Certificate.Handling.Services;

public class ConsumerHostedService(IEventBus eventBus) : BackgroundService
{
    private readonly IEventBus _eventBus = eventBus;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
       await _eventBus.SubscribeAsync<GenerateCertificateEvent, GenerateCertificateEventHandler>(cancellationToken);
    }
}