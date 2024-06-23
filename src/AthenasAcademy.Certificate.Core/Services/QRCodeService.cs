using System.Drawing;
using QRCoder;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using static QRCoder.QRCodeGenerator;

namespace AthenasAcademy.Certificate.Core.Services;

public class QRCodeService : IQRCodeService
{
    private const ECCLevel ECC_LEVEL = ECCLevel.Q;

    public async Task<byte[]> GetQRCode(string text)
    {
        QRCodeGenerator generator = new();
        QRCodeData data = generator.CreateQrCode(text, ECC_LEVEL);
        PngByteQRCode code = new(data);
        return await Task.FromResult(code.GetGraphic(10));
    }
}
