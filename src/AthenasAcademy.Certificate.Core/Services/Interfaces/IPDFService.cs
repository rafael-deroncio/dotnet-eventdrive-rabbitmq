namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

/// <summary>
/// Interface for PDF Service
/// </summary>
public interface IPDFService
{
    /// <summary>
    /// Converts HTML content to a PDF document.
    /// </summary>
    /// <param name="html">The HTML content to be converted.</param>
    /// <returns>A task representing the asynchronous operation, with a byte array result containing the PDF document.</returns>
    Task<byte[]> ConvertHTMLToPDF(string html);
}
