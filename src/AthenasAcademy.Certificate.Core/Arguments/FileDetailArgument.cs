namespace AthenasAcademy.Certificate.Core.Arguments;

public record FileDetailArgument
{
    public int CodeCertificate { get; set; }
    public string File { get; set; }
    public string Type { get; set; }
    public string Path { get; set; }
    public int Size { get; set; }
}
