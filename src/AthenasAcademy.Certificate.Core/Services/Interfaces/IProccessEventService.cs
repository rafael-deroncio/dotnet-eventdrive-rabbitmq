using AthenasAcademy.Certificate.Domain.Requests;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

/// <summary>
/// Interface for Process Event Service
/// </summary>
public interface IProcessEventService
{
    /// <summary>
    /// Generates a certificate based on the provided process ID and certificate request.
    /// </summary>
    /// <param name="process">The process ID associated with the certificate generation.</param>
    /// <param name="request">The request containing the necessary data to generate a certificate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task GenerateCertificate(long process, CertificateRequest request);
}
