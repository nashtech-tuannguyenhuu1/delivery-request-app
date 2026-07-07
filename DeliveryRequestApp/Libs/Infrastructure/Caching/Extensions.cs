using Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Caching;

public static class Extensions
{
    public static IServiceCollection AddHybridCacheManager(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "Redis",
        string instanceName = "myapp:")
    {
        var redisConnectionString =
            configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{connectionStringName}' was not found.");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = instanceName;
        });

        services.AddHybridCache();

        services.AddSingleton<ICacheManager, HybridCacheManager>();

        return services;
    }
}
