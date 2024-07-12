namespace AthenasAcademy.Certificate.Core.Configurations;

public record Parameters
{
    public string BucketName { get; set; }
    public string BucketPathPdf { get; set; }
    public string BucketPathQR { get; set; }
    public string BucketKeyStamp { get; set; }
    public string BucketKeyLogo { get; set; }
    public string BucketKeyTemplate { get; set; }
    
    public int EventMaxAttemps { get; set; }
    public int EventMaxCallbacks { get; set; }
    
    public string DriverDir { get; set; }
}