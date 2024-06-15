using AthenasAcademy.Certificate.Core.Events;
using AthenasAcademy.Certificate.Handling.Handlers;
using AthenasAcademy.Components.EventBus;
using Microsoft.AspNetCore.Builder;

namespace AthenasAcademy.Certificate.Handling.Extensions;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder MapEventBusHandlings(this WebApplication app)
    {

        IEventBus eventBus = app.Services.GetRequiredService<IEventBus>();

        Task.WaitAll(
            [
                eventBus.SubscribeAsync<GenerateCertificateEvent, GenerateCertificateEventHandler>(CancellationToken.None, 1),
                eventBus.SubscribeAsync<TesteCertificateEvent, TesteCertificateEventHandler>(CancellationToken.None, 1)
            ]
        );

        return app;
    }
}