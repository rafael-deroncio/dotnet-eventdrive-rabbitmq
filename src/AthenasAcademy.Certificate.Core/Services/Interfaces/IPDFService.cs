namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface IPDFService
{
    Task<byte[]> ConvertHTMLToPDF(string html);
}
