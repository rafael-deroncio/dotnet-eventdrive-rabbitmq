using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Models;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Requests;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Certificate.Domain.Requests;
using Microsoft.Extensions.Options;

namespace AthenasAcademy.Certificate.Core.Services;

public class ProccessEventService(
    IOptions<Parameters> options,
    ILogger<CertificateService> logger,
    IObjectConverter objectConverter,
    IQRCodeService qrcodeService,
    IHtmlTemplateService templateService,
    IPDFService pdfService,
    IBucketRepository bucketRepository,
    ICertificateRepository certificateRepository
) : IProccessEventService
{
    private readonly Parameters _parameters = options.Value;
    private readonly ILogger<CertificateService> _logger = logger;
    private readonly IObjectConverter _objectConverter = objectConverter;
    private readonly IQRCodeService _qrcodeService = qrcodeService;
    private readonly IHtmlTemplateService _templateService = templateService;
    private readonly IPDFService _pdfService = pdfService;
    private readonly IBucketRepository _bucketRepository = bucketRepository;
    private readonly ICertificateRepository _certificateRepository = certificateRepository;

    public async Task GenerateCertificate(long proccess, CertificateRequest request)
    {
        _logger.LogDebug("Start proccess event for generate certificate.");
        try
        {
            // Apply roles for calculated properties
            request.Course.Workload = request.Course.Disciplines.Sum(x => x.Workload);
            request.Utilization = request.Course.Disciplines.Average(x => x.Utilization);

            // Generate SIGN
            string sign = GetSign(proccess);
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
            _logger.LogError(exception, "Error on proccess event for generate certificate.");
            throw;
        }
        finally
        {
            _logger.LogDebug("Finished proccess event for generate certificate.");
        }
    }

    private async Task GeneratePDF(CertificateParametersRequest parameters, string pathKeyPdf)
    {
        using (StreamReader reader = new(await _bucketRepository.GetFileAsync(_parameters.BucketName, _parameters.BucketKeyTemplate)))
        {
            string template = await reader.ReadToEndAsync();
            string html = await _templateService.GetHtml(parameters, template);

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
            StudentName = FormatStrName(request.Student.Name),
            StudentDocument = FormatStudentDocument(request.Student.Document.Type, request.Student.Document.Number),
            StudentRegistration = FormatRegistration(request.Student.Registration),

            CourseName = FormatStrName(request.Course.Course),
            CourseWorkload = FormatCourseWorkload(request.Course.Workload),
            CourseUtilization = FormatCourseUtilization(request.Utilization),
            CourseConclusionDate = FormatDate(request.Conclusion),

            LogoImageLink = await _bucketRepository.GetDownloadLinkAsync(_parameters.BucketName, _parameters.BucketKeyLogo),
            StampImageLink = await _bucketRepository.GetDownloadLinkAsync(_parameters.BucketName, _parameters.BucketKeyStamp),
            QRCodeImageLink = await _bucketRepository.GetDownloadLinkAsync(_parameters.BucketName, pathQRCode),

            LocationDatetime = FormatLocationDate(request.Conclusion)
        };

    private static string GetSign(long proccess)
    {
        byte[] bytes = Encoding.UTF8.GetBytes($"{proccess}{Guid.NewGuid()}");
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
        => registration.Trim().Length > mask.Length ?
            registration.ToUpper().Trim() :
            registration.ToUpper().Trim().PadLeft(mask.Length, '0');

    private static string FormatStudentDocument(string type, string document)
        => $"{type} - {document.Replace(" ", string.Empty).Trim().ToUpper()}";

    private static string FormatCourseWorkload(int workload)
        => $"{workload}h";

    private static string FormatCourseUtilization(decimal utilization)
        => $"{(int)Math.Round(utilization)}%";

    private static string FormatDate(DateTime date)
        => date.ToString("dd MMMM yyyy", CultureInfo.GetCultureInfo("en-US"));

    private static string FormatLocationDate(DateTime date)
        => $"São Paulo - Brazil, {FormatDate(date)}";
}
