using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using static AthenasAcademy.Certificate.Core.Configurations.PDFOptions;

namespace AthenasAcademy.Certificate.Core.Services;

public class PDFService(
    ILogger<PDFService> logger,
    IOptions<PDFSettings> options
    ) : IPDFService
{
    private readonly ILogger<PDFService> _logger = logger;
    private readonly PDFSettings _settings = options.Value;
    public async Task<byte[]> ConvertHTMLToPDF(string html)
    {
        _logger.LogInformation("Start proccess convert HTML to PDF.");
        try
        {
            return await Task.FromResult(WkhtmltopdfDriver.ConvertHtml(
                _settings.DriverDir,
                _settings.Arguments ?? GetStaticSettings(),
                html));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on convert HTML to PDF.");
            throw;
        }
        finally
        {
            _logger.LogInformation("Finished proccess convert HTML to PDF.");
        }
    }

    private string GetStaticSettings()
        => new PDFOptionsBuilder()
                .SetMargins(top: 5, bottom: 5, left: 5, right: 5)
                .SetZoom("1.2")
                .SetPrintMediaType(true)
                .SetPageWidth(10)
                .SetPageHeight(5.7)
                .Build()
                .ToArgumentsString();
}
