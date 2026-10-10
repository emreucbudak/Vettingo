using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    public class CompanyStatistic : BaseEntity
    {
        public CompanyStatistic()
        {
        }

        public Guid CompanyId { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void CreateCompanyStatistic(Guid companyId)
        {
            CheckCompanyStatisticContent(companyId);
            SetId();
            CompanyId = companyId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public void CheckCompanyStatisticContent(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new ArgumentException("CompanyId boş olamaz.", nameof(companyId));
            }
        }
    }
}
