using Vettingo.AnalyticsService.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Vettingo.AnalyticsService.Infrastructure.Cache
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {
        public async Task<T?> Get<T>(string key)
        {
            var data = await cache.GetAsync(key);
            return data is null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public async Task Remove(string key)
        {
            await cache.RemoveAsync(key);
        }

        public async Task Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var data = JsonSerializer.SerializeToUtf8Bytes(value);

            await cache.SetAsync(key, data, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5),
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1)
            });
        }
    }
}
