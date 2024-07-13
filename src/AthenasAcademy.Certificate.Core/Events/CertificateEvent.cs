using AthenasAcademy.Certificate.EventBus;

namespace AthenasAcademy.Certificate.Core.Events;

public class CertificateEvent : BaseEvent
{
    public int CodeEventProcess { get; set; }
}