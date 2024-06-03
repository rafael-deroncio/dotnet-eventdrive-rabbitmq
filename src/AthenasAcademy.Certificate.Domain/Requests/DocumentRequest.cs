namespace AthenasAcademy.Certificate.Domain.Requests;

public record DocumentRequest
{
    public string Number { get; set; }
    public string Type { get; set; }
}
