namespace AthenasAcademy.Certificate.Domain.Requests;

public record DocumentRequest
{
    public string Type { get; set; }
    public string Number { get; set; }
}
