using AthenasAcademy.Certificate.Handling.Handlers;

namespace AthenasAcademy.Certificate.Handling.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventHandler(this IServiceCollection services)
    {
        services.AddTransient<CertificateEventHandler>();
        return services;
    }
}
