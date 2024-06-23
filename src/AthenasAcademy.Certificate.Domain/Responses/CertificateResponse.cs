namespace AthenasAcademy.Certificate.Domain.Responses;

public record CertificateResponse
{
    public string Student { get; set; }
    public string Course { get; set; }
    public int Workload { get; set; }
    public double Utilization { get; set; }
    public DateTime Completion { get; set; }
    public List<FileResponse> Files { get; set; }
}
