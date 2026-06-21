using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordJobPostingPerformance
{
    public class RecordJobPostingPerformanceCommandHandler(IAnalyticsRepository analyticsRepository, ILogger<RecordJobPostingPerformanceCommandHandler> logger) : IRequestHandler<RecordJobPostingPerformanceCommandRequest, RecordJobPostingPerformanceCommandResponse>
    {
        public async Task<RecordJobPostingPerformanceCommandResponse> Handle(RecordJobPostingPerformanceCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği şirket {CompanyId} ve iş ilanı {JobPostingId} için işleniyor", nameof(RecordJobPostingPerformanceCommandHandler), request.CompanyId, request.JobPostingId);

            var analysis = await analyticsRepository.GetJobPostingPerformanceAnalysisByJobPostingIdAsync(request.JobPostingId);

            if (analysis is null)
            {
                analysis = new Domain.Entities.JobPostingPerformanceAnalysis();
                analysis.CreateJobPostingPerformanceAnalysis(request.CompanyId, request.JobPostingId, request.ViewCount, request.ApplicationCount, request.RecommendationCount, request.HiredRecommendationCount, request.CvViewCount, request.MatchCount, request.TopTenPercentMatchCount, request.AverageCompatibilityRate);
                await analyticsRepository.AddJobPostingPerformanceAnalysisAsync(analysis);
            }
            else
            {
                analysis.UpdateJobPostingPerformanceAnalysis(request.ViewCount, request.ApplicationCount, request.RecommendationCount, request.HiredRecommendationCount, request.CvViewCount, request.MatchCount, request.TopTenPercentMatchCount, request.AverageCompatibilityRate);
                analyticsRepository.UpdateJobPostingPerformanceAnalysis(analysis);
            }

            await analyticsRepository.SaveChangesAsync();

            logger.LogInformation("{HandlerName} isteği tamamlandı. Analiz kimliği: {AnalysisId}", nameof(RecordJobPostingPerformanceCommandHandler), analysis.Id);

            return new RecordJobPostingPerformanceCommandResponse
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
                TopTenPercentMatchRate = analysis.TopTenPercentMatchRate
            };
        }
    }
}

