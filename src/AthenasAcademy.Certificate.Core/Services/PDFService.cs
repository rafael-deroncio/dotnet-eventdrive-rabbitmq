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

    private static string GetStaticSettings()
        => new PDFOptionsBuilder()
                .SetMargins(top: 0, bottom: 0, left: 0, right: 0)
                .SetZoom("1.10")
                .SetDisableSmartShrinking(true)
                .SetDpi("300")
                .SetPrintMediaType(true)
                .SetPageWidth(10)
                .SetPageHeight(6.088)
                .Build()
                .ToArgumentsString();
}
