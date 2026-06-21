using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.AnalyticsService.Application.Repository;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation
{
    public class RecordCandidateRecommendationCommandHandler(IAnalyticsRepository analyticsRepository, ILogger<RecordCandidateRecommendationCommandHandler> logger) : IRequestHandler<RecordCandidateRecommendationCommandRequest, RecordCandidateRecommendationCommandResponse>
    {
        public async Task<RecordCandidateRecommendationCommandResponse> Handle(RecordCandidateRecommendationCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği şirket {CompanyId}, iş ilanı {JobPostingId} ve aday {CandidateId} için işleniyor", nameof(RecordCandidateRecommendationCommandHandler), request.CompanyId, request.JobPostingId, request.CandidateId);

            Domain.Entities.CandidateRecommendationAnalysis analysis = new();
            analysis.CreateCandidateRecommendationAnalysis(request.CompanyId, request.JobPostingId, request.CandidateId, request.CandidateName, request.CompatibilityRate, request.IsHired, request.RecommendedAt, request.HiredAt);

            await analyticsRepository.AddCandidateRecommendationAnalysisAsync(analysis);
            await analyticsRepository.SaveChangesAsync();

            logger.LogInformation("{HandlerName} isteği tamamlandı. Analiz kimliği: {AnalysisId}", nameof(RecordCandidateRecommendationCommandHandler), analysis.Id);

            return new RecordCandidateRecommendationCommandResponse
            {
                Id = analysis.Id,
                CompanyId = analysis.CompanyId,
                JobPostingId = analysis.JobPostingId,
                CandidateId = analysis.CandidateId,
                CandidateName = analysis.CandidateName,
                CompatibilityRate = analysis.CompatibilityRate,
                IsHired = analysis.IsHired,
                RecommendedAt = analysis.RecommendedAt,
                HiredAt = analysis.HiredAt
            };
        }
    }
}

