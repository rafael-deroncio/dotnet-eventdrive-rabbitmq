namespace AthenasAcademy.Certificate.Domain.Requests;

public record CertificateRequest
{
    public StudentRequest Student { get; set; }
    public CourseRequest Course { get; set; }
    public decimal Utilization { get; set; }
    public DateTime ConclusionDate { get; set; }

}
