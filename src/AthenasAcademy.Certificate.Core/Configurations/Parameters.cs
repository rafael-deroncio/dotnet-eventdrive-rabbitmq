namespace AthenasAcademy.Certificate.Core.Configurations;

public record Parameters
{
    public string BucketName { get; set; }
    public string PathPdf { get; set; }
    public string PathPng { get; set; }
    public string PathQR { get; set; }
    public string KeyStamp { get; set; }
    public string KeyLogo { get; set; }
    public string KeyTemplate { get; set; }
    public int Expires { get; set; }
    public int MaxAttempsEvent { get; set; }
}