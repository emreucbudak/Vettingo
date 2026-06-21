namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class CandidateRecommendationAnalysis
    {
        public CandidateRecommendationAnalysis()
        {
        }

        public Guid Id { get; private set; }
        public Guid CompanyId { get; private set; }
        public Guid JobPostingId { get; private set; }
        public Guid CandidateId { get; private set; }
        public string CandidateName { get; private set; } = string.Empty;
        public decimal CompatibilityRate { get; private set; }
        public bool IsHired { get; private set; }
        public DateTime RecommendedAt { get; private set; }
        public DateTime? HiredAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void SetId()
        {
            Id = Guid.CreateVersion7();
        }

        public void CreateCandidateRecommendationAnalysis(
            Guid companyId,
            Guid jobPostingId,
            Guid candidateId,
            string candidateName,
            decimal compatibilityRate,
            bool isHired,
            DateTime recommendedAt,
            DateTime? hiredAt)
        {
            SetId();
            CompanyId = companyId;
            JobPostingId = jobPostingId;
            CandidateId = candidateId;
            CandidateName = candidateName;
            CompatibilityRate = NormalizeRate(compatibilityRate);
            IsHired = isHired;
            RecommendedAt = recommendedAt;
            HiredAt = isHired ? hiredAt : null;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void MarkAsHired(DateTime? hiredAt)
        {
            IsHired = true;
            HiredAt = hiredAt ?? DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        private static decimal NormalizeRate(decimal rate)
        {
            return Math.Clamp(rate, 0m, 100m);
        }
    }
}
