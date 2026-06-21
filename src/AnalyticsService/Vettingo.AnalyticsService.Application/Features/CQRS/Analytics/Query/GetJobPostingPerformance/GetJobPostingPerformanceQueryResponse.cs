namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetJobPostingPerformance
{
    public class GetJobPostingPerformanceQueryResponse
    {
        public Guid Id { get; init; }
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
        public decimal RecommendationHireRate { get; init; }
        public decimal ApplicationToHireRate { get; init; }
        public decimal CvViewToMatchRate { get; init; }
        public decimal TopTenPercentMatchRate { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
