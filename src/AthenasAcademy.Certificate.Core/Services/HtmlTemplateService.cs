using System.Reflection;
using AthenasAcademy.Certificate.Core.Services.Interfaces;

namespace AthenasAcademy.Certificate.Core.Services;

public class HtmlTemplateService : IHtmlTemplateService
{
    public async Task<string> GetHtml(object obj, string template)
    {
        Dictionary<string, string> parameters = ConvertToDictParamters(obj);
        string html = ParseParametersToTemplate(parameters, template);
        return await Task.FromResult(html);
    }

    private static Dictionary<string, string> ConvertToDictParamters(object obj)
    {
        Dictionary<string, string> dict = [];
        Type type = obj.GetType();

        foreach (PropertyInfo property in type.GetProperties())
        {
            string name = property.Name;
            object value = property.GetValue(obj)?.ToString() ?? string.Empty;
            dict.Add("{{" + $"{name.ToUpper()}" + "}}", $"{value}");
        }

        return dict;
    }

    private static string ParseParametersToTemplate(Dictionary<string, string> parameters, string template)
    {
        foreach (KeyValuePair<string, string> parameter in parameters)
        {
            template = template.Replace(parameter.Key, parameter.Value);
        }

        return template;
    }
}
