namespace AthenasAcademy.Certificate.Domain.Requests;

public class CourseRequest
{
    public string Name { get; set; }
    public int Workload {get; set;}
    public List<DisciplineRequest> Subjects { get; set; }
}
