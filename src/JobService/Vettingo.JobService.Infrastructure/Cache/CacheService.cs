using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Vettingo.JobService.Application.Interfaces;

namespace Vettingo.JobService.Infrastructure.Cache
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

        public async Task<T?> Get<T>(string key)
        {
            var data = await cache.GetStringAsync(key);

            return data is null
                ? default
                : JsonSerializer.Deserialize<T>(data, SerializerOptions);
        }

        public Task Remove(string key)
        {
            return cache.RemoveAsync(key);
        }

        public Task Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var data = JsonSerializer.Serialize(value, SerializerOptions);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = expiration ?? TimeSpan.FromMinutes(5),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };

            return cache.SetStringAsync(key, data, cacheOptions);
        }
    }
}
