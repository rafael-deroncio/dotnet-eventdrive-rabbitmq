using System.ComponentModel;

namespace AthenasAcademy.Certificate.Core.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        System.Reflection.FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

        DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

        return attribute == null ? value.ToString() : attribute.Description;
    }
}
