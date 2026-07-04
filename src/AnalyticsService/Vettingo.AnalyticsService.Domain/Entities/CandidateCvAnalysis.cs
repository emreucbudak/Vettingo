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
            CheckCandidateCvAnalysisContent(candidateId, periodStart, periodEnd, aiMatchedJobCount, topTenPercentJobCount, hrViewCount, matchCount, averageMatchRate);
            SetId();
            CandidateId = candidateId;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            AiMatchedJobCount = aiMatchedJobCount;
            TopTenPercentJobCount = topTenPercentJobCount;
            HrViewCount = hrViewCount;
            MatchCount = matchCount;
            AverageMatchRate = averageMatchRate;
            CreatedAt = DateTime.UtcNow;
        }

        public void CheckCandidateCvAnalysisContent(
            Guid candidateId,
            DateTime periodStart,
            DateTime periodEnd,
            int aiMatchedJobCount,
            int topTenPercentJobCount,
            int hrViewCount,
            int matchCount,
            decimal averageMatchRate)
        {
            if (candidateId == Guid.Empty)
            {
                throw new ArgumentException("CandidateId boş olamaz.", nameof(candidateId));
            }

            CheckPeriod(periodStart, periodEnd);
            CheckNonNegative(aiMatchedJobCount, nameof(aiMatchedJobCount));
            CheckNonNegative(topTenPercentJobCount, nameof(topTenPercentJobCount));
            CheckNonNegative(hrViewCount, nameof(hrViewCount));
            CheckNonNegative(matchCount, nameof(matchCount));
            CheckRate(averageMatchRate, nameof(averageMatchRate));
        }

        private static void CheckPeriod(DateTime periodStart, DateTime periodEnd)
        {
            if (periodStart == default)
            {
                throw new ArgumentException("Dönem başlangıç tarihi geçersiz.", nameof(periodStart));
            }

            if (periodEnd == default)
            {
                throw new ArgumentException("Dönem bitiş tarihi geçersiz.", nameof(periodEnd));
            }

            if (periodStart > periodEnd)
            {
                throw new ArgumentException("Dönem başlangıç tarihi dönem bitiş tarihinden sonra olamaz.", nameof(periodStart));
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
