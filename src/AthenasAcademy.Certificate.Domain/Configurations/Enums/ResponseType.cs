using System.ComponentModel;

namespace AthenasAcademy.Certificate.Domain.Configurations.Enums;

public enum ResponseType
{
    [Description("Warning")]
    Warning,

    [Description("Error")]
    Error,

    [Description("Fatal")]
    Fatal,

    [Description("Information")]
    Information,
}