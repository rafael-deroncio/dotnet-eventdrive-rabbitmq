using System.Drawing;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface IQRCodeService
{
    Task<byte[]> GetQRCode(string text);
}
