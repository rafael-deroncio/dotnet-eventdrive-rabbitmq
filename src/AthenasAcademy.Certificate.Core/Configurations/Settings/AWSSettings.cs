namespace AthenasAcademy.Certificate.Core.Configurations.Settings;

public record AWSSettings
{
    public string ServiceURL { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
    public string SignatureVersion { get; set; }
    public bool ForcePathStyle { get; set; }
}