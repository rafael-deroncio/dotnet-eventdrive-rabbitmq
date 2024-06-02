namespace AthenasAcademy.Certificate.Domain.Requests;

public record class DocumentRequest
{
    public string Number { get; set; }
    public string Type { get; set; }
}
