using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Exceptions;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance
{
    public class GetJobPostingPerformanceQueryHandler(IAnalyticsRepository analyticsRepository, ILogger<GetJobPostingPerformanceQueryHandler> logger) : IRequestHandler<GetJobPostingPerformanceQueryRequest, GetJobPostingPerformanceQueryResponse>
    {
        public async Task<GetJobPostingPerformanceQueryResponse> Handle(GetJobPostingPerformanceQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği iş ilanı {JobPostingId} için işleniyor", nameof(GetJobPostingPerformanceQueryHandler), request.JobPostingId);

            var analysis = await analyticsRepository.GetJobPostingPerformanceAnalysisByJobPostingIdAsync(request.JobPostingId);

            if (analysis is null)
            {
                throw new NotFoundException("İş ilanı performans analizi bulunamadı");
            }

            return new GetJobPostingPerformanceQueryResponse
            {
                Id = analysis.Id,
                CompanyId = analysis.CompanyId,
                JobPostingId = analysis.JobPostingId,
                ViewCount = analysis.ViewCount,
                ApplicationCount = analysis.ApplicationCount,
                RecommendationCount = analysis.RecommendationCount,
                HiredRecommendationCount = analysis.HiredRecommendationCount,
                CvViewCount = analysis.CvViewCount,
                MatchCount = analysis.MatchCount,
                TopTenPercentMatchCount = analysis.TopTenPercentMatchCount,
                AverageCompatibilityRate = analysis.AverageCompatibilityRate,
                RecommendationHireRate = analysis.RecommendationHireRate,
                ApplicationToHireRate = analysis.ApplicationToHireRate,
                CvViewToMatchRate = analysis.CvViewToMatchRate,
                TopTenPercentMatchRate = analysis.TopTenPercentMatchRate,
                CreatedAt = analysis.CreatedAt,
                UpdatedAt = analysis.UpdatedAt
            };
        }
    }
}

