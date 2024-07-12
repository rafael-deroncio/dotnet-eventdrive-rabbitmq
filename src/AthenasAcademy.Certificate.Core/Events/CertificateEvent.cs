using AthenasAcademy.Certificate.EventBus;

namespace AthenasAcademy.Certificate.Core.Events;

public class CertificateEvent : BaseEvent
{
    public int CodeEventProccess { get; set; }
}