namespace Vettingo.EvaluationService.Application.Interfaces;

public interface ICacheableQuery : FlashMediator.ICacheableQuery
{
    new string CacheKey { get; set; }
    TimeSpan ExpirationTime { get; set; }

    string FlashMediator.ICacheableQuery.CacheKey => CacheKey;
    TimeSpan? FlashMediator.ICacheableQuery.Expiration => ExpirationTime;
    TimeSpan? FlashMediator.ICacheableQuery.LocalCacheExpiration => ExpirationTime;
    IReadOnlyCollection<string> FlashMediator.ICacheableQuery.CacheTags => [];
}
