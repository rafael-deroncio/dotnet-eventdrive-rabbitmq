using System.Net;
using System.Text.Json;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Exceptions;
using AthenasAcademy.Certificate.Core.Extensions;
using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;
using AthenasAcademy.Components.EventBus;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core.Services;

public class CertificateService(
    IOptions<Parameters> options,
    ILogger<CertificateService> logger,
    IObjectConverter mapper,
    IEventBus eventBus,
    ICertificateRepository certificateRepository,
    IProccessEventRepository proccessEventRepository,
    IBucketRepository bucketRepository
) : ICertificateService
{
    private readonly Parameters _parameters = options.Value;
    private readonly ILogger<CertificateService> _logger = logger;
    private readonly IObjectConverter _mapper = mapper;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICertificateRepository _certificateRepository = certificateRepository;
    private readonly IProccessEventRepository _proccessEventRepository = proccessEventRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;

    public async Task<CertificateResponse> Generate(CertificateRequest request)
    {
        _logger.LogInformation("Start proccess request for generate certificate.");
        try
        {
            CertificateModel certificate = await _certificateRepository.GetCertificateByRegistration(request.Studant.Registration);

            if (certificate != null)
            {
                CertificateResponse response = _mapper.Map<CertificateResponse>(request);
                response.Files = response.Files.Select(file => {
                    file.Download = _bucketRepository.GetDownloadLink(file.Name);
                    return file;
                }).ToList();
                return response;
            }

            CertificateEvent @event = _mapper.Map<CertificateEvent>(request);
            string json = JsonSerializer.Serialize(request);
            @event.CodeEventProccess = await _proccessEventRepository.SaveEventProccess(json);

            await _eventBus.PublishAsync(@event);

            throw new CertificateException(
                title: "Solicitation Proccess",
                message: string.Format("Certificate with register number {0} in proccess. Plase, await!", request.Studant.Registration),
                HttpStatusCode.OK);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on proccess request for generate certificate.");
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished proccess request for generate certificate.");
        }
    }

    public Task<CertificateResponse> Get(string registration, ContentType type)
    {
        _logger.LogInformation("Start proccess request for get certificate {0}.", type.GetName());
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on proccess request for get certificate {0}.", type.GetName());
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished proccess request for get certificate {0}.", type.GetName());
        }
    }

    public async Task ProccessEventGenerateCertificate(CertificateRequest request)
    {
        _logger.LogInformation("Start proccess event for generate certificate.");
        try
        {
            // _logger.LogInformation("Success: {InpudData}", request);
            throw new Exception(string.Format("Error Teste {0}", request));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on proccess event for generate certificate.");
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished proccess event for generate certificate.");
        }
    }
}
