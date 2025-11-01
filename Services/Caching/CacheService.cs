using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace BookStoreEcommerce.Services.Caching;

public class CacheService(
    IDistributedCache _cache,
    IConnectionMultiplexer _redis
) : ICacheService
{
    private readonly IDistributedCache _cache = _cache;
    private readonly IConnectionMultiplexer _redis = _redis;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _jsonOptions);
        var opts = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl };
        await _cache.SetAsync(key, bytes, opts, ct);
    }

    public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        var existing = await GetAsync<T>(key, ct);
        if (existing is not null) return existing;

        var created = await factory(ct);
        await SetAsync(key, created, ttl, ct);
        return created;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        return _cache.RemoveAsync(key, ct);
    }

    public async Task AddListKeyAsync(string collectionKey, string key, CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        await db.SetAddAsync(collectionKey, key);
    }

    public async Task InvalidateListAsync(string collectionKey, CancellationToken ct = default)
    {
        var db = _redis.GetDatabase();
        var members = await db.SetMembersAsync(collectionKey);
        if (members.Length == 0) return;

        var keys = members.Select(m => (RedisKey)m.ToString()).ToArray();
        if (keys.Length > 0) await db.KeyDeleteAsync(keys);

        await db.KeyDeleteAsync(collectionKey); // clear the set
    }

}