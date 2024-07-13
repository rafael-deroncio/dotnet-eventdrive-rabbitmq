using System.Net;
using System.Text.Json;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Exceptions;
using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;
using AthenasAcademy.Certificate.EventBus;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core.Services;

public class CertificateService(
    IOptions<Parameters> options,
    ILogger<CertificateService> logger,
    IObjectConverter mapper,
    IEventBus eventBus,
    ICertificateRepository certificateRepository,
    IProcessEventRepository proccessEventRepository,
    IBucketRepository bucketRepository
) : ICertificateService
{
    private readonly Parameters _parameters = options.Value;
    private readonly ILogger<CertificateService> _logger = logger;
    private readonly IObjectConverter _mapper = mapper;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICertificateRepository _certificateRepository = certificateRepository;
    private readonly IProcessEventRepository _proccessEventRepository = proccessEventRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;

    public async Task<CertificateResponse> CreateCertificate(CertificateRequest request)
    {
        _logger.LogInformation("Start process request for generate certificate.");
        try
        {
            CertificateModel certificate = await _certificateRepository.GetCertificateByRegistration(request.Student.Registration);

            if (certificate != null)
            {
                CertificateResponse response = _mapper.Map<CertificateResponse>(request);
                response.Files = response.Files.Select(file =>
                {
                    file.Download = _bucketRepository.GetDownloadLink(_parameters.BucketName, file.Name);
                    return file;
                }).ToList();
                return response;
            }

            await _eventBus.PublishAsync(new CertificateEvent()
            {
                CodeEventProcess = await _proccessEventRepository.SaveEventProcess(
                    JsonSerializer.Serialize(request)
                ),
            });

            throw new CertificateException(
                title: "Solicitation Process",
                message: string.Format("Certificate with register number {0} in process. Plase, await!", request.Student.Registration),
                HttpStatusCode.OK);
        }
        catch (BaseException) { throw; }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on process request for generate certificate.");
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished process request for generate certificate.");
        }
    }

    public Task<CertificateResponse> GetCertificate(string registration)
    {
        _logger.LogDebug("Start process request for get certificate with registration {0}.", registration);
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on process request for get certificate with registration {0}.", registration);
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished process request for get certificate with registration {0}.", registration);
        }
    }
}
