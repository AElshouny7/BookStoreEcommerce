using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace BookStoreEcommerce.Services.Caching;


public class RedisCacheService(
    IDistributedCache _cache,
    IConnectionMultiplexer _mux

) : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _mux;
    private readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);


    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(key, ct);
        return bytes is null ? default : JsonSerializer.Deserialize<T>(bytes, _json);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, _json);
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

    // ---- List invalidation helpers (use a Redis Set to track list keys) ----
    public async Task AddListKeyAsync(string collectionKey, string key, CancellationToken ct = default)
    {
        var db = _mux.GetDatabase();
        await db.SetAddAsync(collectionKey, key);
    }

    public async Task InvalidateListAsync(string collectionKey, CancellationToken ct = default)
    {
        var db = _mux.GetDatabase();
        var members = await db.SetMembersAsync(collectionKey);
        if (members.Length == 0) return;

        var keys = members.Select(m => (RedisKey)m.ToString()).ToArray();
        if (keys.Length > 0) await db.KeyDeleteAsync(keys);

        await db.KeyDeleteAsync(collectionKey); // clear the set
    }
}