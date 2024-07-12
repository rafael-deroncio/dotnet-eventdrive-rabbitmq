using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record FileDetailModel : CommonDataDTO
{
    public int CodeCertificateFile { get; set; }
    public int CodeCertificate { get; set; }
    public string File { get; set; }
    public string Type { get; set; }
    public string Path { get; set; }
    public int Size { get; set; }
}