namespace AthenasAcademy.Certificate.Core.Arguments;

public record CertificateArgument
{
    public string StudentName { get; set; }
    public string StudentDocument { get; set; }
    public string StudentRegistration { get; set; }
    public string Course { get; set; }
    public DateTime Completion { get; set; }
    public decimal Utilization { get; set; }
    public List<FileDetailArgument> FileDetails { get; set; }
}
