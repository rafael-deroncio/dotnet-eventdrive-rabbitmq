using System.Security.Cryptography;
using System.Text;
using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Helpers;
using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Requests;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core.Services;

public class ProcessEventService(
    IOptions<Parameters> options,
    ILogger<CertificateService> logger,
    IObjectConverter objectConverter,
    IQRCodeService qrcodeService,
    IHtmlTemplateService templateService,
    IPDFService pdfService,
    IBucketRepository bucketRepository,
    ICertificateRepository certificateRepository
) : IProcessEventService
{
    private readonly Parameters _parameters = options.Value;
    private readonly ILogger<CertificateService> _logger = logger;
    private readonly IObjectConverter _objectConverter = objectConverter;
    private readonly IQRCodeService _qrcodeService = qrcodeService;
    private readonly IHtmlTemplateService _templateService = templateService;
    private readonly IPDFService _pdfService = pdfService;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ICertificateRepository _certificateRepository = certificateRepository;

    public async Task GenerateCertificate(long process, CertificateRequest request)
    {
        _logger.LogDebug("Start process event for generate certificate.");
        try
        {
            // Apply roles for calculated properties
            request.Course.Workload = request.Course.Disciplines.Sum(x => x.Workload);
            request.Utilization = request.Course.Disciplines.Average(x => x.Utilization);

            // Generate SIGN
            string sign = GetSign(process);
            string pathQRCode = Path.Combine(_parameters.BucketPathQR, $"{sign}.png");
            string pathKeyPdf = Path.Combine(_parameters.BucketPathPdf, $"{sign}.pdf");

            // Generate QRCode
            await GenerateQRCode(sign, pathQRCode);

            // Generate PDF
            CertificateParametersRequest parameters = await GetParametersForCertificateTemplate(request, pathQRCode);
            await GeneratePDF(parameters, pathKeyPdf);

            // Mount certificate structure
            FileDetailModel pdfFile = await _bucketRepository.GetFileDetailAsync(_parameters.BucketName, pathKeyPdf);
            FileDetailModel qrcFile = await _bucketRepository.GetFileDetailAsync(_parameters.BucketName, pathQRCode);

            CertificateArgument argument = new()
            {
                StudentName = parameters.StudentName,
                StudentDocument = parameters.StudentDocument,
                StudentRegistration = parameters.StudentRegistration,
                Course = parameters.CourseName,

                Sign = sign,
                Workload = request.Course.Workload,
                Conclusion = request.Conclusion,
                Utilization = request.Utilization,

                Files =
                [
                    _objectConverter.Map<FileDetailArgument>(pdfFile),
                    _objectConverter.Map<FileDetailArgument>(qrcFile),
                ],

                Disciplines = request.Course.Disciplines.Select(_objectConverter.Map<DisciplineArgument>).ToList()
            };

            // Save certificate structure
            await _certificateRepository.SaveCertificate(argument);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error on process event for generate certificate.");
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished process event for generate certificate.");
        }
    }

    private async Task GeneratePDF(CertificateParametersRequest parameters, string pathKeyPdf)
    {
        using (StreamReader reader = new(await _bucketRepository.GetFileAsync(_parameters.BucketName, _parameters.BucketKeyTemplate)))
        {
            string template = await reader.ReadToEndAsync();
            string html = await _templateService.ParseParametersToTemplate(parameters, template);

            using (MemoryStream ms = new(await _pdfService.ConvertHTMLToPDF(html)))
            {
                await _bucketRepository.UploadFileAsync(ms, _parameters.BucketName, pathKeyPdf);
            }
        }
    }

    private async Task GenerateQRCode(string sign, string pathQRCode)
    {
        using (MemoryStream ms = new(await _qrcodeService.GetQRCode(sign)))
        {
            await _bucketRepository.UploadFileAsync(ms, _parameters.BucketName, pathQRCode);
        }
    }

    private async Task<CertificateParametersRequest> GetParametersForCertificateTemplate(CertificateRequest request, string pathQRCode)
        => new CertificateParametersRequest
        {
            StudentName = StringHelper.FormatName(request.Student.Name),
            StudentDocument = StringHelper.FormatStudentDocument(request.Student.Document.Type, request.Student.Document.Number),
            StudentRegistration = StringHelper.FormatRegistration(request.Student.Registration),

            CourseName = StringHelper.FormatName(request.Course.Course),
            CourseWorkload = IntegerHelper.FormatCourseWorkload(request.Course.Workload),
            CourseUtilization = DecimalHelper.FormatCourseUtilization(request.Utilization),
            CourseConclusionDate = DateHelper.FormatDate(request.Conclusion),

            LogoImageLink = await _bucketRepository.GetDownloadLinkAsync(_parameters.BucketName, _parameters.BucketKeyLogo),
            StampImageLink = await _bucketRepository.GetDownloadLinkAsync(_parameters.BucketName, _parameters.BucketKeyStamp),
            QRCodeImageLink = await _bucketRepository.GetDownloadLinkAsync(_parameters.BucketName, pathQRCode),

            LocationDatetime = DateHelper.FormatLocationDate(request.Conclusion)
        };

    private static string GetSign(long process)
    {
        byte[] bytes = Encoding.UTF8.GetBytes($"{process}{Guid.NewGuid()}");
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}
