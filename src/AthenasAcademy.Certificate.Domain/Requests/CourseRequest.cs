namespace AthenasAcademy.Certificate.Domain.Requests;

public class CourseRequest
{
    public string Course { get; set; }
    public int Workload {get; set;}
    public List<DisciplineRequest> Disciplines { get; set; }
}
