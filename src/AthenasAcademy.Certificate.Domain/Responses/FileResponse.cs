namespace AthenasAcademy.Certificate.Domain;

public record class FileResponse
{
    public string Name {get; set;}
    public string Download {get; set;}
    public string Type {get; set;}
    public int Size {get; set;}
}
