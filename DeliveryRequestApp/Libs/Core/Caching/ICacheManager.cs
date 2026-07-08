namespace Core.Caching;

public interface ICacheManager
{
    Task<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, int? expirationMinutes = 10, CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, int? expirationMinutes = 10, CancellationToken cancellationToken = default);
}
