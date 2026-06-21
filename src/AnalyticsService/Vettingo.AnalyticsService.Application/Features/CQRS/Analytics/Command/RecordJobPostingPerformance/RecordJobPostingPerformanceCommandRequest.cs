using FlashMediator;

namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordJobPostingPerformance
{
    public record RecordJobPostingPerformanceCommandRequest : IRequest<RecordJobPostingPerformanceCommandResponse>
    {
        public Guid CompanyId { get; init; }
        public Guid JobPostingId { get; init; }
        public int ViewCount { get; init; }
        public int ApplicationCount { get; init; }
        public int RecommendationCount { get; init; }
        public int HiredRecommendationCount { get; init; }
        public int CvViewCount { get; init; }
        public int MatchCount { get; init; }
        public int TopTenPercentMatchCount { get; init; }
        public decimal AverageCompatibilityRate { get; init; }
    }
}
