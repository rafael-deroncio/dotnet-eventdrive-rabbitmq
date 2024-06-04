using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using AthenasAcademy.Certificate.Core.Configurations.Mapper;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.EventBus;
using AthenasAcademy.Certificate.Core.EventBus.Interfaces;
using AthenasAcademy.Certificate.Core.Options;
using AthenasAcademy.Certificate.Core.Repositories.Bucket;
using AthenasAcademy.Certificate.Core.Repositories.Bucket.Interfaces;

namespace AthenasAcademy.Certificate.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
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
            ForcePathStyle = settings.ForcePathStyle
        };

        BasicAWSCredentials credentials = new(settings.AccessKey, settings.SecretKey);

        AmazonS3Client client = new(credentials, config);

        services.AddSingleton<IAmazonS3>(client);

        services.AddSingleton<IBucketRepository, BucketRepository>();

        return services;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEventBusRabbitMQ(configuration);
        return services;
    }

    private static IServiceCollection AddEventBusRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));

        services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
        services.AddSingleton<IEventBus, EventBusRabbitMQ>();

        return services;
    }
}
