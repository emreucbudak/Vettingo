using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCandidateCvAnalysis
{
    public class GetCandidateCvAnalysisQueryHandler(IAnalyticsRepository analyticsRepository, ILogger<GetCandidateCvAnalysisQueryHandler> logger) : IRequestHandler<GetCandidateCvAnalysisQueryRequest, IEnumerable<GetCandidateCvAnalysisQueryResponse>>
    {
        public async Task<IEnumerable<GetCandidateCvAnalysisQueryResponse>> Handle(GetCandidateCvAnalysisQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği aday {CandidateId} için işleniyor", nameof(GetCandidateCvAnalysisQueryHandler), request.CandidateId);

            if (request.LatestOnly)
            {
                var latestAnalysis = await analyticsRepository.GetLatestCandidateCvAnalysisAsync(request.CandidateId);
                return latestAnalysis is null ? [] : [Map(latestAnalysis)];
            }

            var analyses = await analyticsRepository.GetCandidateCvAnalysesByCandidateIdAsync(request.CandidateId);
            return analyses.Select(Map);
        }

        private static GetCandidateCvAnalysisQueryResponse Map(Domain.Entities.CandidateCvAnalysis analysis)
        {
            return new GetCandidateCvAnalysisQueryResponse
            {
                Id = analysis.Id,
                CandidateId = analysis.CandidateId,
                PeriodStart = analysis.PeriodStart,
                PeriodEnd = analysis.PeriodEnd,
                AiMatchedJobCount = analysis.AiMatchedJobCount,
                TopTenPercentJobCount = analysis.TopTenPercentJobCount,
                HrViewCount = analysis.HrViewCount,
                MatchCount = analysis.MatchCount,
                AverageMatchRate = analysis.AverageMatchRate,
                Summary = BuildSummary(analysis),
                CreatedAt = analysis.CreatedAt
            };
        }

        private static string BuildSummary(Domain.Entities.CandidateCvAnalysis analysis)
        {
            var periodDays = Math.Max(1, (analysis.PeriodEnd.Date - analysis.PeriodStart.Date).Days + 1);
            return $"CV'niz son {periodDays} gunde yapay zeka tarafindan {analysis.AiMatchedJobCount} ilanda eslesmeye alindi, {analysis.TopTenPercentJobCount} ilanda ilk %10'luk dilime sokuldu ve {analysis.HrViewCount} IK yetkilisi tarafindan goruntulendi.";
        }
    }
}

