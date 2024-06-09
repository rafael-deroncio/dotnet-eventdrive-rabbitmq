namespace AthenasAcademy.Certificate.API.Configurations.Settings;

public record VersioningSettings
{
    public ApiVersion ApiVersion { get; set; }
    public string Reader { get; set; }
    public Explorer Explorer { get; set; }
}

public record ApiVersion
{
    public int High { get; set; }
    public int Medium { get; set; }
}

public record Explorer
{
    public string Format { get; set; }
}