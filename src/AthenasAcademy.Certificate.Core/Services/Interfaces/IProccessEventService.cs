using AthenasAcademy.Certificate.Domain.Requests;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface IProccessEventService
{
    Task GenerateCertificate(long proccess, CertificateRequest request);
}
