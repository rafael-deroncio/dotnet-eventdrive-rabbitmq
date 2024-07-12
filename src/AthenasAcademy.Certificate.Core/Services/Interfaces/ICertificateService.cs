using AthenasAcademy.Certificate.Domain;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface ICertificateService
{
    Task<CertificateResponse> CreateCertificate(CertificateRequest request);
    Task<CertificateResponse> GetCertificate(string registration);
}
