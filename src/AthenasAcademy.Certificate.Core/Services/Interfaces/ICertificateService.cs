using AthenasAcademy.Certificate.Domain;
using AthenasAcademy.Certificate.Domain.Requests;
using AthenasAcademy.Certificate.Domain.Responses;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface ICertificateService
{
    Task<CertificateResponse> Generate(CertificateRequest request);
    Task<CertificateResponse> Get(string registration, ContentType type);
    Task ProccessEventGenerateCertificate(CertificateRequest request);
}
