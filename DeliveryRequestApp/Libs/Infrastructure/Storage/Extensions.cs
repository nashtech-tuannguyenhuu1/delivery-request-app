using Azure.Storage.Blobs;
using Core.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Storage;

public static class Extensions
{
    public static IServiceCollection AddAzureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BlobStorage")
            ?? throw new InvalidOperationException("Missing 'ConnectionStrings:BlobStorage' configuration value.");

        services.AddSingleton(new BlobServiceClient(connectionString));
        services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();

        return services;
    }
}
