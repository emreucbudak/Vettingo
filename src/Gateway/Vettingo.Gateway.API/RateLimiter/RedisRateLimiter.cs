using Vettingo.Gateway.API.Interface;

namespace Vettingo.Gateway.API.RateLimiter
{
    public class RedisRateLimiter : IRedisRateLimiter
    {
        public Task<bool> CheckRateLimit()
        {
            throw new NotImplementedException();
        }
    }
}
