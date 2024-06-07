namespace AthenasAcademy.Certificate.Core.Arguments;

public record class FileDetailArgument
{
    public int Id { get; set; }
    public string File { get; set; }
    public string Type { get; set; }
    public string Path { get; set; }
    public int Size { get; set; }
}
