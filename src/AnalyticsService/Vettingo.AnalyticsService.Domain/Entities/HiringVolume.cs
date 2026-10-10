using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class HiringVolume : BaseEntity
    {
        public int Year { get; private set; }
        public string Month { get; private set; } = string.Empty;
        public int HireCount { get; private set; }
    }
}
