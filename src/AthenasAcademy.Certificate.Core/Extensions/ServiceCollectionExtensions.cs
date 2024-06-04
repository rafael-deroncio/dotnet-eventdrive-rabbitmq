using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using AthenasAcademy.Certificate.Core.Configurations.Mapper;
using AthenasAcademy.Certificate.Core.Configurations.Mapper.Interfaces;
using AthenasAcademy.Certificate.Core.Configurations.Settings;
using AthenasAcademy.Certificate.Core.EventBus;
using AthenasAcademy.Certificate.Core.EventBus.Interfaces;
using AthenasAcademy.Certificate.Core.Options;

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

        AWSOptions options = configuration.GetAWSOptions();

        string accessKey = configuration["AWS:AccessKey"];
        string secretKey = configuration["AWS:SecretKey"];
        
        options.Credentials = new BasicAWSCredentials(accessKey, secretKey);

        services.AddDefaultAWSOptions(options);
        services.AddAWSService<IAmazonS3>();

        // services.AddSingleton<IBucketRepository, BucketRepository>();

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
