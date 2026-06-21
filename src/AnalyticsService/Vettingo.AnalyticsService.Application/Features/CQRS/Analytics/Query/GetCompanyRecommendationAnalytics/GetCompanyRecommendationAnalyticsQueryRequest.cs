using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics
{
    public class GetCompanyRecommendationAnalyticsQueryRequest : IRequest<GetCompanyRecommendationAnalyticsQueryResponse>
    {
        public Guid CompanyId { get; init; }
    }
}
