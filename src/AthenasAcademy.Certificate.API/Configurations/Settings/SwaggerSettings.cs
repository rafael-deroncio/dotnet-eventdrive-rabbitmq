namespace AthenasAcademy.Certificate.API.Configurations.Settings;

public record SwaggerSettings
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }

    public ContactInfo Contact { get; set; }
}

public record ContactInfo
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Url { get; set; }
}