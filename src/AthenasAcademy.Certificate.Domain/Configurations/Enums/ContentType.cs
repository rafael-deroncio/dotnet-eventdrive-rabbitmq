using System.ComponentModel;

namespace AthenasAcademy.Certificate.Domain;

public enum ContentType
{
    [Description("application/pdf")]
    PDF,

    [Description("image/png")]
    PNG
}
