namespace AthenasAcademy.Certificate.Domain.Requests;

public record class DisciplineRequest
{
    public string Name { get; set; }
    public int Workload { get; set; }
}
