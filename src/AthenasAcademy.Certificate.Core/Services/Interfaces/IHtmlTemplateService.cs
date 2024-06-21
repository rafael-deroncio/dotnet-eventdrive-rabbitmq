using AthenasAcademy.Certificate.Core.Requests;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

public interface IHtmlTemplateService
{
    Task<string> GetHtml(object obj, string template, string qrcodeSign); 
}
