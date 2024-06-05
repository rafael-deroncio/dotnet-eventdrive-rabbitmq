namespace AthenasAcademy.Certificate.Core;

public record ProccessEventModel
{
    public int Process { get; set; }
    public int Status { get; set; }
    public string Error { get; set; }
    public int Attemps { get; set; }
    public string Event { get; set; }
    public bool Finished { get; set; }
  public bool Active { get; set; }
  public DateTime Created { get; set; }
  public DateTime Updated { get; set; }
}
