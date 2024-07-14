using System.Drawing;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

/// <summary>
/// Interface for QR Code Service
/// </summary>
public interface IQRCodeService
{
    /// <summary>
    /// Generates a QR code from the provided text.
    /// </summary>
    /// <param name="text">The text to be encoded in the QR code.</param>
    /// <returns>A task representing the asynchronous operation, with a byte array result containing the QR code image data.</returns>
    Task<byte[]> GetQRCode(string text);
}
