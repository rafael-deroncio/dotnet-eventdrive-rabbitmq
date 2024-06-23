using AthenasAcademy.Components.EventBus;

namespace AthenasAcademy.Certificate.Core.Events;

public class CertificateEvent : BaseEvent
{
    public int CodeEventProccess { get; set; }
    public string Registration { get; set; }
    public string StudentName { get; set; }
    public DateTime StudentBornDate { get; set; }
    public string DocumentNumber { get; set; }
    public string DocumentType { get; set; }
    public string CourseName { get; set; }
    public string CourseWorkload { get; set; }
    public Dictionary<string, int> CourseSubjects { get; set; }
    public decimal Percentage { get; set; }
    public DateTime ConclusionDate { get; set; }

}
