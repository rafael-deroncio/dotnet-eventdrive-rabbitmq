namespace AthenasAcademy.Certificate.Core.Configurations.Settings;

public record SwaggerJwtBearerSettings
{
    public string Name { get; set; }
    public string Scheme { get; set; }
    public string BearerFormat { get; set; }
    public string Description { get; set; }
}