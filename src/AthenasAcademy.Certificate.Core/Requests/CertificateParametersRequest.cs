namespace AthenasAcademy.Certificate.Core.Requests;

public record class CertificateParametersRequest
{
    public string StudantName { get; set; }
    public string StudantDocument { get; set; }
    public string StudantBornDate { get; set; }
    public string StudantRegistration { get; set; }

    public string CourseName { get; set; }
    public string CourseWorkload { get; set; }
    public string CourseUtilization { get; set; }
    public string CourseConslusion { get; set; }

    public string LogoImageLink { get; set; }
    public string StampImageLink { get; set; }
    public string QRCodeImageLink { get; set; }
}
