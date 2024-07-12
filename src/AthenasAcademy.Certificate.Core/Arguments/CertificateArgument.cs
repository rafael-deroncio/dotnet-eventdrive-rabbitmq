namespace AthenasAcademy.Certificate.Core.Arguments;

public record CertificateArgument
{
    public string StudentName { get; set; }
    public string StudentDocument { get; set; }
    public string StudentRegistration { get; set; }
    public string Course { get; set; }
    public int Workload { get; set; }
    public DateTime Conclusion { get; set; }
    public decimal Utilization { get; set; }
    public string Sign { get; set; }
    public List<FileDetailArgument> Files { get; set; }
    public List<DisciplineArgument> Disciplines { get; set; }
}