namespace AthenasAcademy.Certificate.Core.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder MapEventBus(this IApplicationBuilder builder)
    {
        return builder;
    }
}
