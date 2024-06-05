using System.ComponentModel;

namespace AthenasAcademy.Certificate.Core.Configurations.Enums;

public enum EventProcessStatus
{
    [Description("OnProccess")]
    OnProccess = 1,

    [Description("Padding")]
    Padding = 2,

    [Description("Success")]
    Success = 3,

    [Description("Error")]
    Error = 4,
}
