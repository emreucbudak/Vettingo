namespace Vettingo.AnalyticsService.Application.Interfaces
{
    public interface ICacheableQuery : FlashMediator.ICacheableQuery
    {
        public new string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        string FlashMediator.ICacheableQuery.CacheKey => CacheKey;
        TimeSpan? FlashMediator.ICacheableQuery.Expiration => ExpirationTime;
        TimeSpan? FlashMediator.ICacheableQuery.LocalCacheExpiration => ExpirationTime;
        IReadOnlyCollection<string> FlashMediator.ICacheableQuery.CacheTags => [];

    }
}
