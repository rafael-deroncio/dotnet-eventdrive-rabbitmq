namespace AthenasAcademy.Certificate.Domain.Requests;

public record DisciplineRequest
{
    public string Name { get; set; }
    public int Workload { get; set; }
    public double Percentage { get; set; }
    public DateTime ConclusionDate { get; set; }
}
