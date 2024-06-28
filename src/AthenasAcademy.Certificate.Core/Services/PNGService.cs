using AthenasAcademy.Certificate.Core.Services.Interfaces;

namespace AthenasAcademy.Certificate.Core.Services;

public class PNGService : IPNGService
{
    public Task<byte[]> ConvertPDFtoPNG(byte[] pdf)
    {
        throw new NotImplementedException();
    }
}
