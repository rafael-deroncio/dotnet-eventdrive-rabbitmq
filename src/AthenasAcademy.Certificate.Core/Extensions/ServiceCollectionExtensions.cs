using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using AthenasAcademy.Certificate.Core.Configurations;
using AthenasAcademy.Certificate.Core.Configurations.Mapper;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.Repositories.Bucket;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;
using AthenasAcademy.Certificate.Core.Repositories.Postgres.Interfaces;
using AthenasAcademy.Certificate.Core.Services;
using AthenasAcademy.Certificate.Core.Services.Interfaces;
using AthenasAcademy.Components.EventBus;
using AthenasAcademy.Components.EventBus.Brokers;
using AthenasAcademy.Components.EventBus.Brokers.Interfaces;
using Autofac;
using RabbitMQ.Client;

namespace AthenasAcademy.Certificate.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSecrets(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgreSettings>(configuration.GetSection("Postgre"));
        services.Configure<AWSSettings>(configuration.GetSection("AWS"));
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.Configure<PDFSettings>(configuration.GetSection("PDF"));
        return services;
    }

    public static IServiceCollection ConfigureParameters(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Parameters>(configuration.GetSection("Parameters"));
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IHtmlTemplateService, HtmlTemplateService>();
        services.AddScoped<IQRCodeService, QRCodeService>();
        services.AddScoped<IPDFService, PDFService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ICertificateRepository, CertificateRepository>();
        services.AddSingleton<IProccessEventRepository, ProccessEventRepository>();
        return services;
    }

    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        services.AddSingleton<IObjectConverter, ObjectConverter>();
        return services;
    }

    public static IServiceCollection AddAWSS3(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AWSSettings>(configuration.GetSection("AWS"));
        AWSSettings settings = configuration.GetSection("AWS").Get<AWSSettings>();

        AmazonS3Config config = new()
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(settings.Region),
            ServiceURL = settings.ServiceURL,
            ForcePathStyle = settings.ForcePathStyle,
            SignatureVersion = settings.SignatureVersion,
        };

        BasicAWSCredentials credentials = new(settings.AccessKey, settings.SecretKey);

        AmazonS3Client client = new(credentials, config);

        services.AddSingleton<IAmazonS3>(client);

        services.AddSingleton<IBucketRepository, BucketRepository>();

        return services;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMQPersistentConnection(configuration);
        services.AddRabbitMQEventBus(configuration);
        return services;
    }

    private static IServiceCollection AddRabbitMQPersistentConnection(this IServiceCollection services, IConfiguration configuration)
    {
        RabbitMQSettings settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
            ?? throw new ArgumentException();

        services.AddSingleton<IRabbitMQPersistentConnection, RabbitMQPersistentConnection>(provider =>
        {
            ConnectionFactory factory = new()
            {
                HostName = settings.Host,
                Port = settings.Port,
                UserName = settings.Username,
                Password = settings.Password,
                DispatchConsumersAsync = true
            };

            return new RabbitMQPersistentConnection(factory);
        });

        return services;
    }

    private static IServiceCollection AddRabbitMQEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        RabbitMQSettings settings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>()
            ?? throw new ArgumentException();

        services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

        services.AddSingleton<IEventBus, EventBusRabbitMQ>(provider =>
        {
            return new EventBusRabbitMQ(
                provider.GetRequiredService<ILogger<EventBusRabbitMQ>>(),
                provider.GetRequiredService<IRabbitMQPersistentConnection>(),
                provider.GetRequiredService<IEventBusSubscriptionsManager>(),
                provider.GetRequiredService<ILifetimeScope>(),
                settings.Setup.ExchangeBaseName
            );
        });

        return services;
    }
}
