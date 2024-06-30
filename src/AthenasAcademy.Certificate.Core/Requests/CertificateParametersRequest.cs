namespace AthenasAcademy.Certificate.Core.Requests;

public record class CertificateParametersRequest
{
    public string StudentName { get; set; }
    public string StudentDocument { get; set; }
    public string StudentBornDate { get; set; }
    public string StudentRegistration { get; set; }

    public string CourseName { get; set; }
    public string CourseWorkload { get; set; }
    public string CourseUtilization { get; set; }
    public string CourseConslusion { get; set; }

    public string LogoImageLink { get; set; }
    public string StampImageLink { get; set; }
    public string QRCodeImageLink { get; set; }

    public string LocationToday { get; set; }
}
