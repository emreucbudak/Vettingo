namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateCvAnalysis
{
    public class RecordCandidateCvAnalysisCommandResponse
    {
        public Guid Id { get; init; }
        public Guid CandidateId { get; init; }
        public DateTime PeriodStart { get; init; }
        public DateTime PeriodEnd { get; init; }
        public int AiMatchedJobCount { get; init; }
        public int TopTenPercentJobCount { get; init; }
        public int HrViewCount { get; init; }
        public int MatchCount { get; init; }
        public decimal AverageMatchRate { get; init; }
        public string Summary { get; init; } = string.Empty;
    }
}
