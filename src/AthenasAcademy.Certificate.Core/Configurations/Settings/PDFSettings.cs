namespace AthenasAcademy.Certificate.Core.Configurations.Settings;

public record PDFSettings
{
    public string DriverDir { get; set; }
    public string Arguments { get; set; }
}
