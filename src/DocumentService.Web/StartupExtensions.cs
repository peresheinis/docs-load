using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using DocumentService.Core;
using DocumentService.Core.Repositories;
using DocumentService.Infrastructure;
using DocumentService.Infrastructure.Repositories;
using DocumentService.Web.Configurations;
using DocumentService.Web.Extensions;
using DocumentService.Web.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DocumentService.Web;

public static class StartupExtensions
{
    public static void SetupMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    }

    public static void SetupDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.SetupNpgsql<DatabaseContext>(builder.Configuration);
        builder.Services.AddScoped<IUnitOfWork, UnitOfWorkPostgres>();
        builder.Services.AddScoped<IFilesRepository, FilesRepository>();
    }

    public static void SetupAuth(this WebApplicationBuilder builder)
    {
        builder.Services.SetupAuthentication(builder.Configuration);
    }

    public static void SetupS3Storage(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration
            .GetSection(StorageConfiguration.Storage)
            .Get<StorageConfiguration>();

        builder.Services.Configure<StorageConfiguration>(builder.Configuration
            .GetSection(StorageConfiguration.Storage));

        AWSConfigsS3.UseSignatureVersion4 = true;

        var amazonS3Config = new AmazonS3Config() { RegionEndpoint = RegionEndpoint.GetBySystemName(configuration.Region), ServiceURL = configuration.Url };
        var aWSCredentials = new BasicAWSCredentials(configuration.AccessKey, configuration.SecretKey);
        var amazonClient = new AmazonS3Client(aWSCredentials, amazonS3Config);

        builder.Services.AddScoped(c => amazonClient);
    }

    public static void SetupMapper(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(MapperEntitiesProfile));
    }

    public static void SetupLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog();
        SeqSetup.SetupLogging(builder.Environment, builder.Configuration);
    }

    public static void UseMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        database.Database.Migrate();
    }
}