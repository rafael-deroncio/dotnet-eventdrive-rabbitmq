namespace AthenasAcademy.Certificate.Domain.Requests;

public record class CertificateRequest
{
    public StudantRequest Studant { get; set; }
    public CourseRequest Course { get; set; }
    public decimal Percentage { get; set; }
    public DateTime ConclusionDate { get; set; }

}
