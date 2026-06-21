namespace Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyRecommendationAnalytics
{
    public class GetCompanyRecommendationAnalyticsQueryResponse
    {
        public Guid CompanyId { get; init; }
        public int RecommendedCandidateCount { get; init; }
        public int HiredRecommendedCandidateCount { get; init; }
        public decimal AverageCompatibilityRate { get; init; }
        public decimal HiringRate { get; init; }
        public IEnumerable<CompanyRecommendationAnalyticsItemResponse> Recommendations { get; init; } = [];
    }

    public class CompanyRecommendationAnalyticsItemResponse
    {
        public Guid Id { get; init; }
        public Guid JobPostingId { get; init; }
        public Guid CandidateId { get; init; }
        public string CandidateName { get; init; } = string.Empty;
        public decimal CompatibilityRate { get; init; }
        public bool IsHired { get; init; }
        public DateTime RecommendedAt { get; init; }
        public DateTime? HiredAt { get; init; }
    }
}
