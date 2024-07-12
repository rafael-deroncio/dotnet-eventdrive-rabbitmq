namespace AthenasAcademy.Certificate.Domain.Requests;

public record DisciplineRequest
{
    public string Discipline { get; set; }
    public int Workload { get; set; }
    public decimal Utilization { get; set; }
    public DateTime Conclusion { get; set; }
}
