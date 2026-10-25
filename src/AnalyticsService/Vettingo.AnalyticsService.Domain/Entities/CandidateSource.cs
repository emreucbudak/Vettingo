using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class CandidateSource : BaseEntity
    {
        public CandidateSource()
        {
            SetId();
            CreatedAt = DateTime.UtcNow;
        }

        public int Application { get; private set; }
        public int Scout { get; private set; }
        public int CandidateCount { get; private set; }
    }
}
