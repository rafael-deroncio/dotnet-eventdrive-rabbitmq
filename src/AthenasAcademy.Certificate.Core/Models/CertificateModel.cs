using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record CertificateModel : CommonDataDTO
{
    public string Student { get; set; }
    public string Document { get; set; }
    public string Registration { get; set; }
    public string Course { get; set; }
    public DateTime Completion { get; set; }
    public decimal Utilization { get; set; }
}
