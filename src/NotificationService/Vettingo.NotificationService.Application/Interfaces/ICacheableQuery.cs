namespace Vettingo.NotificationService.Application.Interfaces
{
    public interface ICacheableQuery : FlashMediator.ICacheableQuery
    {
        public new string CacheKey { get; set; }
        public TimeSpan ExpireTime { get; set; }

        string FlashMediator.ICacheableQuery.CacheKey => CacheKey;
        TimeSpan? FlashMediator.ICacheableQuery.Expiration => ExpireTime;
        TimeSpan? FlashMediator.ICacheableQuery.LocalCacheExpiration => ExpireTime;
        IReadOnlyCollection<string> FlashMediator.ICacheableQuery.CacheTags => [];
    }
}
