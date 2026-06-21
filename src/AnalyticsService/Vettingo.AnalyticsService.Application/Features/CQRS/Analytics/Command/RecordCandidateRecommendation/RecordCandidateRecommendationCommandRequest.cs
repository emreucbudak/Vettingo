using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation
{
    public record RecordCandidateRecommendationCommandRequest : IRequest<RecordCandidateRecommendationCommandResponse>
    {
        public Guid CompanyId { get; init; }
        public Guid JobPostingId { get; init; }
        public Guid CandidateId { get; init; }
        public string CandidateName { get; init; } = string.Empty;
        public decimal CompatibilityRate { get; init; }
        public bool IsHired { get; init; }
        public DateTime RecommendedAt { get; init; } = DateTime.UtcNow;
        public DateTime? HiredAt { get; init; }
    }
}
