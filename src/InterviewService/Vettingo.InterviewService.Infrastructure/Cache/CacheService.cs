using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.Infrastructure.Cache
{
    public sealed class CacheService(IDistributedCache cache) : ICacheService
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

        public async Task Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            var data = JsonSerializer.Serialize(value, SerializerOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            await cache.SetStringAsync(key, data, options);
        }
    }
}
