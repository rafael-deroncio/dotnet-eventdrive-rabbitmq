namespace AthenasAcademy.Certificate.Domain.Responses;

public record class DetailsResponse
{
    public string Studant { get; set; }
    public string Course { get; set; }
    public int Workload { get; set; }
    public decimal Percentage { get; set; }
    public DateTime ConclusionDate { get; set; }
}
