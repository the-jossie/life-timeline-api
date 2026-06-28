using System.Text.Json;
using StackExchange.Redis;

public class CacheService
{
    private readonly IDatabase _db;

    public CacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiration);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;

        return JsonSerializer.Deserialize<T>(value.ToString());
    }

    public async Task InvalidateAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
