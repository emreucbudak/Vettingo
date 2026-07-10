namespace Vettingo.NotificationService.Application.Interfaces
{
    public interface ICacheableQuery
    {
        public string CacheKey { get; set; }
        public TimeSpan ExpireTime { get; set; }
    }
}
