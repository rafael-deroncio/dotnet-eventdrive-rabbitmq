namespace AthenasAcademy.Certificate.Domain.Requests;

public record class CertificateRequest
{
    public StudantRequest Studant {get; set;}
    public CourseRequest Course {get; set;}
    public decimal CompletentionPercentage {get; set;}
    public DateTime CompletentionDate {get; set;}

}
