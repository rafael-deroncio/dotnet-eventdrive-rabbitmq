using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record CertificateFileModel : CommonDataDTO
{
    public List<FileDetailModel> Details { get; set; }
}
