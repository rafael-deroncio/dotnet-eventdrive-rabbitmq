using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Extensions;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;
using AthenasAcademy.Components.EventBus;

namespace AthenasAcademy.Certificate.Core.Services;

public class CertificateService(
    ILogger<CertificateService> logger,
    IObjectConverter mapper,
    IEventBus eventBus,
    ICertificateRepository certificateRepository,
    IBucketRepository bucketRepository
) : ICertificateService
{
    private readonly ILogger<CertificateService> _logger = logger;
    private readonly IObjectConverter _mapper = mapper;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICertificateRepository _certificateRepository = certificateRepository;
    private readonly IBucketRepository _bucketRepository = bucketRepository;

    public async Task<CertificateResponse> Generate(CertificateRequest request)
    {
        _logger.LogInformation("Start proccess request for generate certificate.");
        try
        {
            await _eventBus.PublishAsync(new TesteCertificateEvent() { CodeEventProccess = new Random().Next() });
            await _eventBus.PublishAsync(new GenerateCertificateEvent() { CodeEventProccess = new Random().Next() });
            return new();
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
            var filename = $"{type.GetName().ToLower()}/teste.pdf";
            var arquivo = _bucketRepository.GetFile(filename);
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

    public Task ProccessEventGenerateCertificate(CertificateRequest request)
    {
        _logger.LogInformation("Start proccess event for generate certificate.");
        try
        {
            throw new NotImplementedException();
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
