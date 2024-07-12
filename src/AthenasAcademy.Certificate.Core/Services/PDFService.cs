using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;
using static AthenasAcademy.Certificate.Core.Configurations.PDFOptions;

namespace AthenasAcademy.Certificate.Core.Services;

public class PDFService(
    IOptions<Parameters> options
    ) : IPDFService
{
    private readonly Parameters _settings = options.Value;
    public async Task<byte[]> ConvertHTMLToPDF(string html)
    {
        return await Task.FromResult(WkhtmltopdfDriver.ConvertHtml(
            _settings.DriverDir,
            GetStaticSettings(),
            html));
    }

    private static string GetStaticSettings()
        => new PDFOptionsBuilder()
                .SetMargins(top: 0, bottom: 0, left: 0, right: 0)
                .SetZoom("1.10")
                .SetDisableSmartShrinking(true)
                .SetDpi("600")
                .SetPrintMediaType(true)
                .SetPageWidth(10)
                .SetPageHeight(6.12)
                .Build()
                .ToArgumentsString();
}
