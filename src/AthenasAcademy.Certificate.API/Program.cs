using AthenasAcademy.Certificate.API.Extensions;
using AthenasAcademy.Certificate.Core.Extensions;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add Serilog Configuration
builder.Host.UseSerilog();

// Add use of secrets yaml file
builder.Host.UseSecrets();

// Add Custom Cors Policies
builder.Services.AddCors(builder.Configuration);

// Add API Explorer
builder.Services.AddEndpointsApiExplorer();

// Add Custom API Versioning
builder.Services.AddApiVersioning(builder.Configuration);

// // Add Custom Swagger e UI
builder.Services.AddSwagger(builder.Configuration);

// // Add Custom Swagger Auth with JWT Bearer
builder.Services.AddSwaggerJwtBearer(builder.Configuration);

// Add URLs lowercase
builder.Services.AddLowerCaseRouting();

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

// Add Custom Authentication
builder.Services.AddAuthentication(builder.Configuration);

// Add Authorization
builder.Services.AddAuthorization();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();

app.UseCors();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseApiVersioning();

app.UseSwagger(builder.Configuration);

app.UseGlobalExceptionHandler();

app.UseHsts();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
