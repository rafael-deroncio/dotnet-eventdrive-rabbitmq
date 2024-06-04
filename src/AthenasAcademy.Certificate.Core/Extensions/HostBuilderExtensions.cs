using AthenasAcademy.Certificate.Core.Configurations.Logger;
using Microsoft.Extensions.Configuration.Yaml;
using Serilog;
using Serilog.Exceptions;

namespace AthenasAcademy.Certificate.Core.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, config) =>
            config.MinimumLevel.Information()
                  .MinimumLevel.ControlledBy(LoggingLevelSwitcher._instance)
                  .Enrich.With(new CustomEnricher(context.Configuration["AppDetails:Name"]))
                  .Enrich.WithProperty("Application", context.Configuration["ApplicationName"])
                  .Enrich.WithProperty("Envioroment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                  .Enrich.FromLogContext()
                  .Enrich.WithExceptionDetails()
                  .WriteTo.Console()
                  );
        return hostBuilder;
    }

    public static IHostBuilder UseSecrets(this IHostBuilder hostBuilder, string file = "secrets.yaml")
    {
        hostBuilder.ConfigureAppConfiguration((context, config) => 
            {
                config.Sources.Clear();
                config.AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true);
                config.AddYamlFile(path: file, optional: false, reloadOnChange: true);
            }
        );
        return hostBuilder;
    }
}
