using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core;

public record EventProccessModel : CommonDataDTO
{
    public int Status { get; set; }
    public string Error { get; set; }
    public int Attemps { get; set; }
    public string Json { get; set; }
    public bool Finished { get; set; }
}