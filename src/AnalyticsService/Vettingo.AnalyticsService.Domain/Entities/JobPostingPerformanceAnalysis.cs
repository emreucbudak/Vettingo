namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class JobPostingPerformanceAnalysis
    {
        public JobPostingPerformanceAnalysis()
        {
        }

        public Guid Id { get; private set; }
        public Guid CompanyId { get; private set; }
        public Guid JobPostingId { get; private set; }
        public int ViewCount { get; private set; }
        public int ApplicationCount { get; private set; }
        public int RecommendationCount { get; private set; }
        public int HiredRecommendationCount { get; private set; }
        public int CvViewCount { get; private set; }
        public int MatchCount { get; private set; }
        public int TopTenPercentMatchCount { get; private set; }
        public decimal AverageCompatibilityRate { get; private set; }
        public decimal RecommendationHireRate { get; private set; }
        public decimal ApplicationToHireRate { get; private set; }
        public decimal CvViewToMatchRate { get; private set; }
        public decimal TopTenPercentMatchRate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateJobPostingPerformanceAnalysis(
            Guid companyId,
            Guid jobPostingId,
            int viewCount,
            int applicationCount,
            int recommendationCount,
            int hiredRecommendationCount,
            int cvViewCount,
            int matchCount,
            int topTenPercentMatchCount,
            decimal averageCompatibilityRate)
        {
            SetId();
            CompanyId = companyId;
            JobPostingId = jobPostingId;
            UpdateJobPostingPerformanceAnalysis(viewCount, applicationCount, recommendationCount, hiredRecommendationCount, cvViewCount, matchCount, topTenPercentMatchCount, averageCompatibilityRate);
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void UpdateJobPostingPerformanceAnalysis(
            int viewCount,
            int applicationCount,
            int recommendationCount,
            int hiredRecommendationCount,
            int cvViewCount,
            int matchCount,
            int topTenPercentMatchCount,
            decimal averageCompatibilityRate)
        {
            ViewCount = Math.Max(0, viewCount);
            ApplicationCount = Math.Max(0, applicationCount);
            RecommendationCount = Math.Max(0, recommendationCount);
            HiredRecommendationCount = Math.Max(0, hiredRecommendationCount);
            CvViewCount = Math.Max(0, cvViewCount);
            MatchCount = Math.Max(0, matchCount);
            TopTenPercentMatchCount = Math.Max(0, topTenPercentMatchCount);
            AverageCompatibilityRate = NormalizeRate(averageCompatibilityRate);
            RecommendationHireRate = CalculateRate(HiredRecommendationCount, RecommendationCount);
            ApplicationToHireRate = CalculateRate(HiredRecommendationCount, ApplicationCount);
            CvViewToMatchRate = CalculateRate(MatchCount, CvViewCount);
            TopTenPercentMatchRate = CalculateRate(TopTenPercentMatchCount, MatchCount);
            UpdatedAt = DateTime.UtcNow;
        }

        private static decimal CalculateRate(int numerator, int denominator)
        {
            if (denominator <= 0)
            {
                return 0m;
            }

            return Math.Round(numerator * 100m / denominator, 2);
        }

        private static decimal NormalizeRate(decimal rate)
        {
            return Math.Clamp(rate, 0m, 100m);
        }
    }
}
