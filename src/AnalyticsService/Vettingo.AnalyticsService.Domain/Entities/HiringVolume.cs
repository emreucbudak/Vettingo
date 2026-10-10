using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class HiringVolume : BaseEntity
    {
        public int Year { get; private set; }
        public int HireCount { get; private set; }
    }
}
