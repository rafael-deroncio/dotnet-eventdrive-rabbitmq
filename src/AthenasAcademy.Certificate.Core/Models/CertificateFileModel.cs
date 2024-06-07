using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record CertificateFileModel : CommonDataDTO
{
    public CertificateModel Certificate { get; set; }
    public List<FileDetailModel> Files { get; set; }
}
