using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Core.Exceptions;
using AthenasAcademy.Certificate.Core.Extensions;
using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Requests;
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
                response.Files = response.Files.Select(file =>
                {
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
            // generate sign
            string sign = GetSign();
            string pathQR = Path.Combine(_parameters.PathQR, $"{sign}.png");

            // generate html
            CertificateParametersRequest certificateParameters = new()
            {
                StudantName = FormatStrName(request.Studant.Name),
                StudantDocument = FormatStudantDocument(request.Studant.Document.Type, request.Studant.Document.Number),
                StudantBornDate = FormatDate(request.Studant.BornDate),
                StudantRegistration = FormatRegistration(request.Studant.Registration),

                CourseName = FormatStrName(request.Course.Name),
                CourseWorkload = FormatCourseWorkload(request.Course.Workload),
                CourseUtilization = FormatCourseUtilization(request.Percentage),
                CourseConslusion = FormatDate(request.ConclusionDate),

                LogoImageLink = _bucketRepository.GetDownloadLink(_parameters.KeyLogo),
                StampImageLink = _bucketRepository.GetDownloadLink(_parameters.KeyStamp),
                QRCodeImageLink = _bucketRepository.GetDownloadLink(pathQR)
            };

            // generate pdf

            // generate png            
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

    private static string GetSign()
    {
        string guid = Guid.NewGuid().ToString();
        byte[] bytes = Encoding.UTF8.GetBytes(guid);
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    private static string FormatStrName(string str)
    {
        string formated = string.Join(" ", str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim())).ToLower();
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(formated);
    }

    private static string FormatRegistration(string registration, string mask = "0000000000")
        =>  registration.Trim().Length > mask.Length ?
            registration.ToUpper().Trim() :
            registration.ToUpper().Trim().PadLeft(mask.Length, '0');

    private static string FormatStudantDocument(string type, string document) 
        => $"{type} - {document.Replace(" ", string.Empty).Trim().ToUpper()}";

    private static string FormatCourseWorkload(int workload) 
        => $"{workload} hours";

    private static string FormatCourseUtilization(double utilization) 
        => string.Format("{0:0.0}", utilization).Replace(",", ".");

    private static string FormatDate(DateTime date) 
        => date.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("en-US"));
}
