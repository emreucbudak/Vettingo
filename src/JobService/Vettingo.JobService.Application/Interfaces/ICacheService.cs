namespace Vettingo.JobService.Application.Interfaces
{
    public interface ICacheService
    {
        Task Set<T>(string key, T value, TimeSpan? expiration = null);
        Task Remove(string key);
        Task<T?> Get<T>(string key);
    }
}
