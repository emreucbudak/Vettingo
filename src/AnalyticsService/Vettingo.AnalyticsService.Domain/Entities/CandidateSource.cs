using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class CandidateSource : BaseEntity
    {
        public int Application { get; private set; }
        public int Scout { get; private set; }
    }
}
