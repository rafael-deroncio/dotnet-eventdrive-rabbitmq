using System.Text.Json;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Enums;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.EventBus;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Handling.Handlers;

public class CertificateEventHandler(
    ILogger<CertificateEventHandler> logger,
    IOptions<Parameters> options,
    IProccessEventService proccessEventService,
    IProccessEventRepository proccessEventRepository
    ) : IEventHandler<CertificateEvent>
{
    private readonly ILogger<CertificateEventHandler> _logger = logger;
    private readonly Parameters _parameters = options.Value;
    private readonly IProccessEventService _proccessEventService = proccessEventService;
    private readonly IProccessEventRepository _proccessEventRepository = proccessEventRepository;


    public async Task Handle(CertificateEvent @event)
    {
        if (!await _proccessEventRepository.EventInProccess(@event.CodeEventProccess))
        {
            _logger.LogInformation("Start proccess {ProccessNumber} event {EventName} to generate certificate.", @event.CodeEventProccess, nameof(CertificateEvent));

            try
            {
                using (_logger.BeginScope(GetScopeLogger(@event)))
                {
                    await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.OnProccess);

                    string json = await _proccessEventRepository.GetEventProccess(@event.CodeEventProccess);
                    CertificateRequest request = JsonSerializer.Deserialize<CertificateRequest>(json);
                    await _proccessEventService.GenerateCertificate(@event.CodeEventProccess, request);

                    await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.Success, finish: true);
                }
            }
            catch (Exception ex)
            {
                if (!await _proccessEventRepository.MaximumAttemptsReached(@event.CodeEventProccess, _parameters.EventMaxAttemps))
                    await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.Padding, ex.Message);
                else
                    await _proccessEventRepository.UpdateEventProccess(@event.CodeEventProccess, EventProcessStatus.Error, ex.Message, finish: true);
                throw;
            }
            finally
            {
                _logger.LogInformation("Finished proccess '{ProccessNumber}' event '{EventName}'.", @event.CodeEventProccess, nameof(CertificateEvent));
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
            ["CodeEventProccess"] = @event.CodeEventProccess
        };
    }
}
