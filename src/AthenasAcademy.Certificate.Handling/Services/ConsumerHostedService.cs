using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Handling.Handlers;
using AthenasAcademy.Certificate.EventBus;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Handling.Services;

public class ConsumerHostedService(
    IOptions<Parameters> options,
    IEventBus eventBus) : BackgroundService
{
    private readonly Parameters _parameters = options.Value;
    private readonly IEventBus _eventBus = eventBus;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        List<Task> consumers =
        [
            _eventBus.SubscribeAsync<CertificateEvent, CertificateEventHandler>(cancellationToken, _parameters.EventMaxAttemps)
        ];

        await Task.WhenAll(consumers);
    }
}