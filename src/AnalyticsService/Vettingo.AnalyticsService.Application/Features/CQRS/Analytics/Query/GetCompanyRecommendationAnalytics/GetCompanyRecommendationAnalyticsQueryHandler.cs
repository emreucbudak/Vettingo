using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics
{
    public class GetCompanyRecommendationAnalyticsQueryHandler(IAnalyticsRepository analyticsRepository, ILogger<GetCompanyRecommendationAnalyticsQueryHandler> logger) : IRequestHandler<GetCompanyRecommendationAnalyticsQueryRequest, GetCompanyRecommendationAnalyticsQueryResponse>
    {
        public async Task<GetCompanyRecommendationAnalyticsQueryResponse> Handle(GetCompanyRecommendationAnalyticsQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği şirket {CompanyId} için işleniyor", nameof(GetCompanyRecommendationAnalyticsQueryHandler), request.CompanyId);

            var recommendations = (await analyticsRepository.GetCandidateRecommendationAnalysesByCompanyIdAsync(request.CompanyId)).ToList();
            var recommendedCount = recommendations.Count;
            var hiredCount = recommendations.Count(recommendation => recommendation.IsHired);

            return new GetCompanyRecommendationAnalyticsQueryResponse
            {
                CompanyId = request.CompanyId,
                RecommendedCandidateCount = recommendedCount,
                HiredRecommendedCandidateCount = hiredCount,
                AverageCompatibilityRate = recommendedCount == 0 ? 0m : Math.Round(recommendations.Average(recommendation => recommendation.CompatibilityRate), 2),
                HiringRate = recommendedCount == 0 ? 0m : Math.Round(hiredCount * 100m / recommendedCount, 2),
                Recommendations = recommendations.Select(recommendation => new CompanyRecommendationAnalyticsItemResponse
                {
                    Id = recommendation.Id,
                    JobPostingId = recommendation.JobPostingId,
                    CandidateId = recommendation.CandidateId,
                    CandidateName = recommendation.CandidateName,
                    CompatibilityRate = recommendation.CompatibilityRate,
                    IsHired = recommendation.IsHired,
                    RecommendedAt = recommendation.RecommendedAt,
                    HiredAt = recommendation.HiredAt
                })
            };
        }
    }
}

