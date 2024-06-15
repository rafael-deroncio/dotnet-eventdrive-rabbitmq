using AthenasAcademy.Components.EventBus;

namespace AthenasAcademy.Certificate.Core.Events;

public class TesteCertificateEvent : BaseEvent
{
    public int CodeEventProccess { get; set; }
}
