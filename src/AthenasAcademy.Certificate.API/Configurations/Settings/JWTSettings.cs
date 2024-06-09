namespace AthenasAcademy.Certificate.API.Configurations.Settings;

public record JWTSettings
{
    public byte[] IssuerSigningKey { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool RequireHttpsMetadata { get; set; }
    public bool SaveToken { get; set; }
    public string SymmetricSecurityKey { get; set; }
}