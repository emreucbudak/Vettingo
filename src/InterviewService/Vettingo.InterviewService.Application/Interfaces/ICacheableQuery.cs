namespace Vettingo.InterviewService.Application.Interfaces
{
    public interface ICacheableQuery : FlashMediator.ICacheableQuery
    {
        public new string CacheKey { get; }
        public TimeSpan ExpirationTime { get; }

        string FlashMediator.ICacheableQuery.CacheKey => CacheKey;
        TimeSpan? FlashMediator.ICacheableQuery.Expiration => ExpirationTime;
        TimeSpan? FlashMediator.ICacheableQuery.LocalCacheExpiration => ExpirationTime;
        IReadOnlyCollection<string> FlashMediator.ICacheableQuery.CacheTags => [];
    }
}
