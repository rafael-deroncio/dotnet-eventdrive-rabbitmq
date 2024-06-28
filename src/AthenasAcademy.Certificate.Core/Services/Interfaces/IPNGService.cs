namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface IPNGService
{
    Task<byte[]> ConvertPDFtoPNG(byte[] pdf);
}
