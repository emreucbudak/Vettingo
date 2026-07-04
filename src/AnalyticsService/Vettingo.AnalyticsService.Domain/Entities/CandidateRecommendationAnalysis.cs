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
            CheckCandidateRecommendationAnalysisContent(companyId, jobPostingId, candidateId, candidateName, compatibilityRate, isHired, recommendedAt, hiredAt);
            SetId();
            CompanyId = companyId;
            JobPostingId = jobPostingId;
            CandidateId = candidateId;
            CandidateName = candidateName;
            CompatibilityRate = compatibilityRate;
            IsHired = isHired;
            RecommendedAt = recommendedAt;
            HiredAt = isHired ? hiredAt ?? DateTime.UtcNow : null;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void MarkAsHired(DateTime? hiredAt)
        {
            CheckHiredAt(RecommendedAt, hiredAt);
            IsHired = true;
            HiredAt = hiredAt ?? DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CheckCandidateRecommendationAnalysisContent(
            Guid companyId,
            Guid jobPostingId,
            Guid candidateId,
            string candidateName,
            decimal compatibilityRate,
            bool isHired,
            DateTime recommendedAt,
            DateTime? hiredAt)
        {
            CheckGuid(companyId, nameof(companyId));
            CheckGuid(jobPostingId, nameof(jobPostingId));
            CheckGuid(candidateId, nameof(candidateId));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(candidateName, nameof(candidateName));
            CheckRate(compatibilityRate, nameof(compatibilityRate));

            if (recommendedAt == default)
            {
                throw new ArgumentException("Önerilme tarihi geçersiz.", nameof(recommendedAt));
            }

            if (isHired)
            {
                CheckHiredAt(recommendedAt, hiredAt);
            }
        }

        private static void CheckGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"{parameterName} boş olamaz.", parameterName);
            }
        }

        private static void CheckRate(decimal rate, string parameterName)
        {
            if (rate is < 0m or > 100m)
            {
                throw new ArgumentOutOfRangeException(parameterName, rate, "Oran 0 ile 100 arasında olmalıdır.");
            }
        }

        private static void CheckHiredAt(DateTime recommendedAt, DateTime? hiredAt)
        {
            if (hiredAt.HasValue && hiredAt.Value == default)
            {
                throw new ArgumentException("İşe alınma tarihi geçersiz.", nameof(hiredAt));
            }

            if (hiredAt.HasValue && hiredAt.Value < recommendedAt)
            {
                throw new ArgumentException("İşe alınma tarihi önerilme tarihinden önce olamaz.", nameof(hiredAt));
            }
        }
    }
}
