using FlashMediator;

using Vettingo.AnalyticsService.Application.Interfaces;
namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics
{
    public class GetCompanyRecommendationAnalyticsQueryRequest : IRequest<GetCompanyRecommendationAnalyticsQueryResponse>, ICacheableQuery
    {
        public Guid CompanyId { get; init; }

        public GetCompanyRecommendationAnalyticsQueryRequest(Guid companyId)
        {
            CompanyId = companyId;
            CacheKey = CompanyId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(10);
        }

        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
