namespace AthenasAcademy.Certificate.Domain.Responses;

public record CertificateResponse
{
    public Guid Identifier { get; set; }
    public DetailsResponse Details { get; set; }
    public FileResponse File { get; set; }
}
