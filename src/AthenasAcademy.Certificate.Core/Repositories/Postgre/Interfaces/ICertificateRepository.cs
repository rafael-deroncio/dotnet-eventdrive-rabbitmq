using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Core.Models;

namespace AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;

/// <summary>
/// Interface for Certificate Repository
/// </summary>
public interface ICertificateRepository
{
    /// <summary>
    /// Saves the certificate.
    /// </summary>
    /// <param name="argument">The certificate argument containing the necessary data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveCertificate(CertificateArgument argument);

    /// <summary>
    /// Gets the certificate by registration.
    /// </summary>
    /// <param name="registration">The registration identifier of the certificate.</param>
    /// <returns>A task representing the asynchronous operation, with a CertificateModel as the result.</returns>
    Task<CertificateModel> GetCertificateByRegistration(string registration);
}
