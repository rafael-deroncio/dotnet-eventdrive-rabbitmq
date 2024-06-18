using System.ComponentModel;

namespace AthenasAcademy.Certificate.Core.Configurations.Enums;

public enum EventProcessStatus
{
    [Description("Padding")]
    Padding = 1,

    [Description("OnProccess")]
    OnProccess = 2,

    [Description("Success")]
    Success = 3,

    [Description("Error")]
    Error = 4,
}
