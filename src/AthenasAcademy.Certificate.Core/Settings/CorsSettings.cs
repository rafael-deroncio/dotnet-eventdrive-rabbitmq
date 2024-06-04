namespace AthenasAcademy.Certificate.Core.Configurations.Settings;

public record CorsSettings
{
    public CorsPolicy Policy { get; set; }
}

public record CorsPolicy
{
    public string Name { get; set; }
    public string[] AllowedOrigins { get; set; }
    public string[] AllowedMethods { get; set; }
    public string[] AllowedHeaders { get; set; }
}