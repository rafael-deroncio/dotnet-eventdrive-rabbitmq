namespace AthenasAcademy.Certificate.Domain.Requests;

public record StudentRequest
{
    public string Name { get; set; }
    public DateTime BornDate { get; set; }
    public string Registration { get; set; }
    public DocumentRequest Document { get; set; }
}
