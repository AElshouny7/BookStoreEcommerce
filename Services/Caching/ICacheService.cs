namespace BookStoreEcommerce.Services.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);
    Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task AddListKeyAsync(string collectionKey, string key, CancellationToken ct = default);
    Task InvalidateListAsync(string collectionKey, CancellationToken ct = default);
}