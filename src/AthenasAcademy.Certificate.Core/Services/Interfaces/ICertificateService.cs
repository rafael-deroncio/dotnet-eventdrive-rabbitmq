using AthenasAcademy.Certificate.Domain;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

/// <summary>
/// Interface for Certificate Service
/// </summary>
public interface ICertificateService
{
    /// <summary>
    /// Creates a new certificate based on the provided request.
    /// </summary>
    /// <param name="request">The request containing the necessary data to create a certificate.</param>
    /// <returns>A task representing the asynchronous operation, with a CertificateResponse as the result.</returns>
    Task<CertificateResponse> CreateCertificate(CertificateRequest request);

    /// <summary>
    /// Retrieves a certificate based on the registration identifier.
    /// </summary>
    /// <param name="registration">The registration identifier of the certificate.</param>
    /// <returns>A task representing the asynchronous operation, with a CertificateResponse as the result.</returns>
    Task<CertificateResponse> GetCertificate(string registration);
}
