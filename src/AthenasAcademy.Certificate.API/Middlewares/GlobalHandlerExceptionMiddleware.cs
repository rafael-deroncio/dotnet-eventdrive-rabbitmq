using System.Net;
using System.Text.Json;
using AthenasAcademy.Certificate.Domain.Configurations.Enums;
using AthenasAcademy.Certificate.Core.Extensions;
using AthenasAcademy.Certificate.Core.Exceptions;
using AthenasAcademy.Certificate.Domain.Responses;

namespace AthenasAcademy.Certificate.API.Middlewares;

public class GlobalHandlerExceptionMiddleware
{
    private readonly ILogger<GlobalHandlerExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public GlobalHandlerExceptionMiddleware(RequestDelegate next, ILogger<GlobalHandlerExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            ExceptionResponse response = new()
            {
                Type = ex.Type.GetDescription(),
                Title = ex.Title,
                Messages = [ex.Message]
            };

            string json = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)ex.Code;

            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on procces request.");
            ExceptionResponse response = new()
            {
                Title = "Internal Error",
                Type = ResponseType.Fatal.GetDescription(),
                Messages = ["An error occurred while processing the request."]
            };

            string json = JsonSerializer.Serialize(response);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(json);
        }
    }
}