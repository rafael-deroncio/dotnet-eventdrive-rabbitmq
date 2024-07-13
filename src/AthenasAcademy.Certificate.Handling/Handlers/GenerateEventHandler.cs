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
    IProcessEventService proccessEventService,
    IProcessEventRepository proccessEventRepository
    ) : IEventHandler<CertificateEvent>
{
    private readonly ILogger<CertificateEventHandler> _logger = logger;
    private readonly Parameters _parameters = options.Value;
    private readonly IProcessEventService _proccessEventService = proccessEventService;
    private readonly IProcessEventRepository _proccessEventRepository = proccessEventRepository;


    public async Task Handle(CertificateEvent @event)
    {
        if (!await _proccessEventRepository.EventInProcess(@event.CodeEventProcess))
        {
            _logger.LogInformation("Start process {ProcessNumber} event {EventName} to generate certificate.", @event.CodeEventProcess, nameof(CertificateEvent));

            try
            {
                using (_logger.BeginScope(GetScopeLogger(@event)))
                {
                    await _proccessEventRepository.UpdateEventProcess(@event.CodeEventProcess, EventProcessStatus.OnProcess);

                    string json = await _proccessEventRepository.GetEventProcess(@event.CodeEventProcess);
                    CertificateRequest request = JsonSerializer.Deserialize<CertificateRequest>(json);
                    await _proccessEventService.GenerateCertificate(@event.CodeEventProcess, request);

                    await _proccessEventRepository.UpdateEventProcess(@event.CodeEventProcess, EventProcessStatus.Success, finish: true);
                }
            }
            catch (Exception ex)
            {
                if (!await _proccessEventRepository.MaximumAttemptsReached(@event.CodeEventProcess, _parameters.EventMaxAttemps))
                    await _proccessEventRepository.UpdateEventProcess(@event.CodeEventProcess, EventProcessStatus.Padding, ex.Message);
                else
                    await _proccessEventRepository.UpdateEventProcess(@event.CodeEventProcess, EventProcessStatus.Error, ex.Message, finish: true);
                throw;
            }
            finally
            {
                _logger.LogInformation("Finished process '{ProcessNumber}' event '{EventName}'.", @event.CodeEventProcess, nameof(CertificateEvent));
            }
        }
        else
            throw new Exception("Event 'CertificateEvent' in process.");
    }

    private static Dictionary<string, object> GetScopeLogger<TEvent>(TEvent @event)
    where TEvent : CertificateEvent
    {
        return new Dictionary<string, object>
        {
            ["GuidEventProcess"] = @event.Id,
            ["CodeEventProcess"] = @event.CodeEventProcess
        };
    }
}
