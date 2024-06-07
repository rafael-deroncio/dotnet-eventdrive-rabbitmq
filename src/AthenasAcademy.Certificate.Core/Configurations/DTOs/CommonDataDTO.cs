namespace AthenasAcademy.Certificate.Core.Configurations.DTOs;

public record CommonDataDTO
{
    public int Id { get; set; }
    public bool Active { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
