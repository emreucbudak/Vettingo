namespace Vettingo.ExamService.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T?> Get<T>(string key);
        Task Set<T>(string key, T value, TimeSpan? expiration = null);
        Task Remove(string key);
    }
}
