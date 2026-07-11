using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Vettingo.EvaluationService.Application.Interfaces;

namespace Vettingo.EvaluationService.Infrastructure.Cache;

public sealed class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> Get<T>(string key)
    {
        byte[]? data = await cache.GetAsync(key);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public Task Remove(string key)
    {
        return cache.RemoveAsync(key);
    }

    public async Task Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(value);

        await cache.SetAsync(key, data, new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
        });
    }
}
