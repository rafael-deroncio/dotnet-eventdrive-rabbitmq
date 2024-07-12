namespace AthenasAcademy.Certificate.Core.Arguments;

public record DisciplineArgument
{
    public int CodeCertificate { get; set; }
    public string Discipline { get; set; }
    public int Workload { get; set; }
    public decimal Utilization { get; set; }
    public DateTime Conclusion { get; set; }
}