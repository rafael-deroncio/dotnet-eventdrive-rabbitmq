using AthenasAcademy.Certificate.Core.Arguments;
using AthenasAcademy.Certificate.Core.Models;

namespace AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;

public interface ICertificateRepository
{
    Task<CertificateModel> SaveCertificate(CertificateArgument argumet);
    Task<CertificateModel> GetCertificateByRegistration(string registration);
}
