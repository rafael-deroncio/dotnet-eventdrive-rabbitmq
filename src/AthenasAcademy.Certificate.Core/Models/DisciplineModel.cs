using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record class DisciplineModel : CommonDataDTO
{
    public int CodeDiscipline { get; set; }
    public int CodeCertificate { get; set; }
    public string Discipline { get; set; }
    public double Utilization { get; set; }
}
