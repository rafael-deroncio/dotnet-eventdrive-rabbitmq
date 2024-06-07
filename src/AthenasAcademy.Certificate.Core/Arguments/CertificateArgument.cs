namespace AthenasAcademy.Certificate.Core.Arguments;

public record CertificateArgument
{
    public int Id { get; set; }
    public string Student { get; set; }
    public string Document { get; set; }
    public string Registration { get; set; }
    public string Course { get; set; }
    public DateTime Completion { get; set; }
    public decimal Utilization { get; set; }
    public List<FileDetailArgument> FileDetails { get; set; }
}
