namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation
{
    public class RecordCandidateRecommendationCommandResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }
        public Guid JobPostingId { get; init; }
        public Guid CandidateId { get; init; }
        public string CandidateName { get; init; } = string.Empty;
        public decimal CompatibilityRate { get; init; }
        public bool IsHired { get; init; }
        public DateTime RecommendedAt { get; init; }
        public DateTime? HiredAt { get; init; }
    }
}
