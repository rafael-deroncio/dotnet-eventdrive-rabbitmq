using AthenasAcademy.Components.EventBus;

namespace AthenasAcademy.Certificate.Core.Events;

public class GenerateCertificateEvent : BaseEvent
{
    public int CodeEventProccess { get; set; }
}
