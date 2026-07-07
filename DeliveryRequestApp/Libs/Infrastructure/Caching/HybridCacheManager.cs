using Core.Caching;
using Microsoft.Extensions.Caching.Hybrid;

namespace Infrastructure.Caching;
public class HybridCacheManager : ICacheManager
{
    private const int DefaultExpirationMinutes = 10;

    private readonly HybridCache _cache;

    public HybridCacheManager(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        int? expirationMinutes = DefaultExpirationMinutes,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(factory);

        return await _cache.GetOrCreateAsync(
            key: key,
            factory: ct => new ValueTask<T>(factory(ct)),
            options: CreateOptions(expirationMinutes),
            tags: null,
            cancellationToken: cancellationToken);
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            return await _cache.GetOrCreateAsync<T>(
                key: key,
                factory: static _ => ValueTask.FromException<T>(
                    new CacheMissException()),
                cancellationToken: cancellationToken);
        }
        catch (CacheMissException)
        {
            return default;
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        int? expirationMinutes = DefaultExpirationMinutes,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        await _cache.SetAsync(
            key: key,
            value: value,
            options: CreateOptions(expirationMinutes),
            tags: null,
            cancellationToken: cancellationToken);
    }

    private static HybridCacheEntryOptions CreateOptions(int? expirationMinutes)
    {
        var minutes = expirationMinutes ?? DefaultExpirationMinutes;

        if (minutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expirationMinutes),
                "Expiration minutes must be greater than zero.");
        }

        var expiration = TimeSpan.FromMinutes(minutes);

        return new HybridCacheEntryOptions
        {
            Expiration = expiration,
            LocalCacheExpiration = expiration
        };
    }

    private sealed class CacheMissException : Exception;
}
