using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;

public sealed record GetCompanyHiringVolumeQueryRequest(Guid CompanyId)
    : IRequest<IEnumerable<GetCompanyHiringVolumeQueryResponse>>, ICacheableQuery
{
    public string CacheKey => $"analytics:company:{CompanyId}:hiring-volume";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    public TimeSpan? LocalCacheExpiration => TimeSpan.FromMinutes(10);
    public IReadOnlyCollection<string> CacheTags => [$"analytics:company:{CompanyId}:statistics"];
}
