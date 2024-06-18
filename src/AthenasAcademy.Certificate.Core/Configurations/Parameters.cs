namespace AthenasAcademy.Certificate.Core.Configurations;

public record Parameters
{
    public string PathPdf { get; set; }
    public string PathPng { get; set; }
    public int MaxAttempsEvent { get; set; }
}