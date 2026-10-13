using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class HiringVolume : BaseEntity
    {
        public Guid CompanyStatisticId { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public int HireCount { get; private set; }
    }
}
