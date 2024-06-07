using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core;

public record FileDetailModel : CommonDataDTO
{
    public string File { get; set; }
    public string Type { get; set; }
    public string Path { get; set; }
    public int Size { get; set; }
}