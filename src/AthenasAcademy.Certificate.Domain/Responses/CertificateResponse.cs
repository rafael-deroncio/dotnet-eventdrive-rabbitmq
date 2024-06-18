namespace AthenasAcademy.Certificate.Domain.Responses;

public record CertificateResponse
{
    public string Studant { get; set; }
    public string Course { get; set; }
    public int Workload { get; set; }
    public decimal Utilization { get; set; }
    public DateTime Completion { get; set; }
    public List<FileResponse> Files { get; set; }
}
