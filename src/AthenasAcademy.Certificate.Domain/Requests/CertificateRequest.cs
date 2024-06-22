namespace AthenasAcademy.Certificate.Domain.Requests;

public record CertificateRequest
{
    public StudantRequest Studant { get; set; }
    public CourseRequest Course { get; set; }
    public double Percentage { get; set; }
    public DateTime ConclusionDate { get; set; }

}
