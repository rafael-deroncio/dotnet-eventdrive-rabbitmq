using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Components.EventBus;

namespace AthenasAcademy.Certificate.Handling.Handlers;

public class GenerateCertificateEventHandler(
    ILogger<GenerateCertificateEventHandler> logger,
    IObjectConverter mapper,
    ICertificateService certificateService
    ) : IEventHandler<GenerateCertificateEvent>
{
    private readonly ILogger<GenerateCertificateEventHandler> _logger = logger;
    private readonly IObjectConverter _mapper = mapper;
    private readonly ICertificateService _certificateService = certificateService;

    public async Task Handle(GenerateCertificateEvent @event)
    {
        using (_logger.BeginScope(typeof(GenerateCertificateEvent).Name, GetScopeLogger(@event)))
        {
            CertificateRequest request = _mapper.Map<CertificateRequest>(@event);
            await _certificateService.ProccessEventGenerateCertificate(request);
        }
    }

    private static Dictionary<string, object> GetScopeLogger<TEvent>(TEvent @event)
    where TEvent : GenerateCertificateEvent
    {
        return new Dictionary<string, object>
        {
            ["GuidEventProccess"] = @event.Id,
            ["ProccessEventCode"] = @event.CodeEventProccess
        };
    }
}
