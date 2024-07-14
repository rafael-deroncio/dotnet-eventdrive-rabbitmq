using AthenasAcademy.Certificate.Core.Requests;

namespace AthenasAcademy.Certificate.Core.Services.Interfaces;

/// <summary>
/// Interface for HTML Template Service
/// </summary>
public interface IHtmlTemplateService
{
    /// <summary>
    /// Generates HTML content based on the provided object and template.
    /// </summary>
    /// <param name="obj">The object containing the data to be used in the template.</param>
    /// <param name="template">The HTML template string.</param>
    /// <returns>A task representing the asynchronous operation, with a string result containing the generated HTML content.</returns>
    Task<string> ParseParametersToTemplate(object obj, string template); 
}
