using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateCvAnalysis
{
    public class RecordCandidateCvAnalysisCommandHandler(IAnalyticsRepository analyticsRepository, ILogger<RecordCandidateCvAnalysisCommandHandler> logger) : IRequestHandler<RecordCandidateCvAnalysisCommandRequest, RecordCandidateCvAnalysisCommandResponse>
    {
        public async Task<RecordCandidateCvAnalysisCommandResponse> Handle(RecordCandidateCvAnalysisCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği aday {CandidateId} için işleniyor", nameof(RecordCandidateCvAnalysisCommandHandler), request.CandidateId);

            Domain.Entities.CandidateCvAnalysis analysis = new();
            analysis.CreateCandidateCvAnalysis(request.CandidateId, request.PeriodStart, request.PeriodEnd, request.AiMatchedJobCount, request.TopTenPercentJobCount, request.HrViewCount, request.MatchCount, request.AverageMatchRate);

            await analyticsRepository.AddCandidateCvAnalysisAsync(analysis);
            await analyticsRepository.SaveChangesAsync();

            logger.LogInformation("{HandlerName} isteği tamamlandı. Analiz kimliği: {AnalysisId}", nameof(RecordCandidateCvAnalysisCommandHandler), analysis.Id);

            return new RecordCandidateCvAnalysisCommandResponse
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
                Summary = BuildSummary(analysis)
            };
        }

        private static string BuildSummary(Domain.Entities.CandidateCvAnalysis analysis)
        {
            var periodDays = Math.Max(1, (analysis.PeriodEnd.Date - analysis.PeriodStart.Date).Days + 1);
            return $"CV'niz son {periodDays} gunde yapay zeka tarafindan {analysis.AiMatchedJobCount} ilanda eslesmeye alindi, {analysis.TopTenPercentJobCount} ilanda ilk %10'luk dilime sokuldu ve {analysis.HrViewCount} IK yetkilisi tarafindan goruntulendi.";
        }
    }
}

