using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record CertificateModel : CommonDataDTO
{
    public int CodeCertificate { get; set; }
    public string StudentName { get; set; }
    public string StudentDocument { get; set; }
    public string StudentRegistration { get; set; }
    public string Course { get; set; }
    public int Workload { get; set; }
    public decimal Utilization { get; set; }
    public DateTime Conclusion { get; set; }
    public List<FileDetailModel> Files { get; set; }
    public List<DisciplineModel> Disciplines { get; set; }
}
