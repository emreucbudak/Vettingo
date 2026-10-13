using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyCandidateSource;

public sealed record GetCompanyCandidateSourceQueryRequest(Guid CompanyId)
    : IRequest<GetCompanyCandidateSourceQueryResponse>, ICacheableQuery
{
    public string CacheKey => $"analytics:company:{CompanyId}:candidate-source";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(10);
    public TimeSpan? LocalCacheExpiration => TimeSpan.FromMinutes(10);
    public IReadOnlyCollection<string> CacheTags => [$"analytics:company:{CompanyId}:statistics"];
}
