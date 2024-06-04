using Serilog.Core;
using Serilog.Events;

namespace AthenasAcademy.Certificate.Core.Configurations.Logger;

public class CustomEnricher : ILogEventEnricher
{
    private readonly string _applicationValue;

    public CustomEnricher(string applicationValue)
    {
        _applicationValue = applicationValue;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var applicationProperty = new LogEventProperty("ServiceName", new ScalarValue(_applicationValue));
        logEvent.AddOrUpdateProperty(applicationProperty);
    }
}
