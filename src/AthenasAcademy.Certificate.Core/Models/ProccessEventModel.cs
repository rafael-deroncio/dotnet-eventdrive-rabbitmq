using AthenasAcademy.Certificate.Core.Configurations.DTOs;

namespace AthenasAcademy.Certificate.Core.Models;

public record ProccessEventModel : CommonDataDTO
{
    public int CodeProccessEvent { get; set; }
    public int CodeStatus { get; set; }
    public string Error { get; set; }
    public int Attemps { get; set; }
    public string Event { get; set; }
    public bool Finished { get; set; }
}
