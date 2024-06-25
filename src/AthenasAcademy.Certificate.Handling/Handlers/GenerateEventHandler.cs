using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Enums;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Components.EventBus;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Handling.Handlers;

public class CertificateEventHandler(
    IOptions<Parameters> options,
    IObjectConverter mapper,
    ICertificateService certificateService,
    IProccessEventRepository proccessEventRepository
    ) : IEventHandler<CertificateEvent>
{
    private readonly Parameters _parameters = options.Value;
    private readonly IObjectConverter _mapper = mapper;
    private readonly ICertificateService _certificateService = certificateService;
    private readonly IProccessEventRepository _proccessEventRepository = proccessEventRepository;


    public async Task Handle(CertificateEvent @event)
    {
        if (!await _proccessEventRepository.EventInProccess(@event.CodeEventProccess))
        {
            try
            {
                await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.OnProccess);

                CertificateRequest request = _mapper.Map<CertificateRequest>(@event);
                await _certificateService.ProccessEventGenerateCertificate(request);

                await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.Success);
            }
            catch (Exception ex)
            {
                if (!await _proccessEventRepository.MaximumAttemptsReached(@event.CodeEventProccess, _parameters.EventMaxAttemps))
                    await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.Padding, ex.Message);
                else
                    await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.Error, ex.Message, finish: true);
                throw;
            }
        }
        else
            throw new Exception("Event 'CertificateEvent' in proccess.");
    }

    private static Dictionary<string, object> GetScopeLogger<TEvent>(TEvent @event)
    where TEvent : CertificateEvent
    {
        return new Dictionary<string, object>
        {
            ["GuidEventProccess"] = @event.Id,
            ["ProccessEventCode"] = @event.CodeEventProccess
        };
    }
}
