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
            CheckJobPostingPerformanceAnalysisContent(companyId, jobPostingId, viewCount, applicationCount, recommendationCount, hiredRecommendationCount, cvViewCount, matchCount, topTenPercentMatchCount, averageCompatibilityRate);
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
            CheckJobPostingPerformanceAnalysisContent(CompanyId, JobPostingId, viewCount, applicationCount, recommendationCount, hiredRecommendationCount, cvViewCount, matchCount, topTenPercentMatchCount, averageCompatibilityRate);
            ViewCount = viewCount;
            ApplicationCount = applicationCount;
            RecommendationCount = recommendationCount;
            HiredRecommendationCount = hiredRecommendationCount;
            CvViewCount = cvViewCount;
            MatchCount = matchCount;
            TopTenPercentMatchCount = topTenPercentMatchCount;
            AverageCompatibilityRate = averageCompatibilityRate;
            RecommendationHireRate = CalculateRate(HiredRecommendationCount, RecommendationCount);
            ApplicationToHireRate = CalculateRate(HiredRecommendationCount, ApplicationCount);
            CvViewToMatchRate = CalculateRate(MatchCount, CvViewCount);
            TopTenPercentMatchRate = CalculateRate(TopTenPercentMatchCount, MatchCount);
            UpdatedAt = DateTime.UtcNow;
        }

        public void CheckJobPostingPerformanceAnalysisContent(
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
            CheckGuid(companyId, nameof(companyId));
            CheckGuid(jobPostingId, nameof(jobPostingId));
            CheckNonNegative(viewCount, nameof(viewCount));
            CheckNonNegative(applicationCount, nameof(applicationCount));
            CheckNonNegative(recommendationCount, nameof(recommendationCount));
            CheckNonNegative(hiredRecommendationCount, nameof(hiredRecommendationCount));
            CheckNonNegative(cvViewCount, nameof(cvViewCount));
            CheckNonNegative(matchCount, nameof(matchCount));
            CheckNonNegative(topTenPercentMatchCount, nameof(topTenPercentMatchCount));
            CheckRate(averageCompatibilityRate, nameof(averageCompatibilityRate));
        }

        private static decimal CalculateRate(int numerator, int denominator)
        {
            if (denominator <= 0)
            {
                return 0m;
            }

            return Math.Round(numerator * 100m / denominator, 2);
        }

        private static void CheckGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }

        private static void CheckNonNegative(int value, string parameterName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, value, "Değer negatif olamaz.");
            }
        }

        private static void CheckRate(decimal rate, string parameterName)
        {
            if (rate is < 0m or > 100m)
            {
                throw new ArgumentOutOfRangeException(parameterName, rate, "Oran 0 ile 100 arasında olmalıdır.");
            }
        }
    }
}
