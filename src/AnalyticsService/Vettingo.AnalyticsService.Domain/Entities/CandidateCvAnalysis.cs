namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class CandidateCvAnalysis
    {
        public CandidateCvAnalysis()
        {
        }

        public Guid Id { get; private set; }
        public Guid CandidateId { get; private set; }
        public DateTime PeriodStart { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        public int AiMatchedJobCount { get; private set; }
        public int TopTenPercentJobCount { get; private set; }
        public int HrViewCount { get; private set; }
        public int MatchCount { get; private set; }
        public decimal AverageMatchRate { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateCandidateCvAnalysis(
            Guid candidateId,
            DateTime periodStart,
            DateTime periodEnd,
            int aiMatchedJobCount,
            int topTenPercentJobCount,
            int hrViewCount,
            int matchCount,
            decimal averageMatchRate)
        {
            SetId();
            CandidateId = candidateId;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            AiMatchedJobCount = Math.Max(0, aiMatchedJobCount);
            TopTenPercentJobCount = Math.Max(0, topTenPercentJobCount);
            HrViewCount = Math.Max(0, hrViewCount);
            MatchCount = Math.Max(0, matchCount);
            AverageMatchRate = Math.Clamp(averageMatchRate, 0m, 100m);
            CreatedAt = DateTime.UtcNow;
        }
    }
}
