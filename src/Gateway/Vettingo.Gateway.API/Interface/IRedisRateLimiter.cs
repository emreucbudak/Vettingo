namespace Vettingo.Gateway.API.Interface
{
    public interface IRedisRateLimiter
    {
        Task<bool> CheckRateLimit();
    }
}
