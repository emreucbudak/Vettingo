using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class CandidateStatistic : BaseEntity
    {
        public CandidateStatistic()
        {
        }

        public Guid CandidateId { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void CreateCandidateStatistic(Guid candidateId)
        {
            CheckCandidateStatisticContent(candidateId);
            SetId();
            CandidateId = candidateId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void CheckCandidateStatisticContent(Guid candidateId)
        {
            if (candidateId == Guid.Empty)
            {
                throw new ArgumentException("CandidateId boş olamaz.", nameof(candidateId));
            }
        }
    }
}
