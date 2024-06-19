using AthenasAcademy.Certificate.Core.Extensions;
using AthenasAcademy.Certificate.Handling.Extensions;
using AthenasAcademy.Certificate.Handling.Services;
using Autofac.Extensions.DependencyInjection;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Use Scope in Tasks 
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add Serilog Configuration
builder.Host.UseSerilog();

// Add use of secrets yaml file
builder.Host.UseSecrets();

// Configure secrets in IOptions
builder.Services.ConfigureSecrets(builder.Configuration);
builder.Services.ConfigureParameters(builder.Configuration);

// Add Services DI
builder.Services.AddServices();

// Add Repositories DI
builder.Services.AddRepositories();

// Add AutoMapper DI
builder.Services.AddAutoMapper();

// Add AWS S3 DI
builder.Services.AddAWSS3(builder.Configuration);

// Add RabbitMQ DI
builder.Services.AddEventBus(builder.Configuration);

// Add Event Handlers DI
builder.Services.AddEventHandler();

builder.Services.AddHostedService<ConsumerHostedService>();

builder.Build().Run();