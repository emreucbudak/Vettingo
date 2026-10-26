using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Domain.Common;

namespace Vettingo.AnalyticsService.Domain.Entities
{
    [Index(nameof(CompanyId))]
    public class CompanyStatistic : BaseEntity
    {
        public CompanyStatistic()
        {
        }

        public CompanyStatistic(Guid companyId)
        {
            CreateCompanyStatistic(companyId);
            CandidateSource = new CandidateSource();
            CandidateSourceId = CandidateSource.Id;
        }

        public Guid CompanyId { get; private set; }
        public int AverageFeedback { get; private set; }
        public int TotalJobPostingCount { get; private set; }
        public int ActiveJobPostingCount { get; private set; }
        public int TotalCandidatesInterviewed { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public Guid CandidateSourceId { get; private set; }
        public CandidateSource CandidateSource { get; private set; } = null!;
        public ICollection<HiringVolume> HiringVolumes { get; private set; } = new List<HiringVolume>();

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

        public IReadOnlyCollection<HiringVolume> GetLatestHiringVolumes()
        {
            return HiringVolumes
                .OrderBy(volume => volume.CreatedAt)
                .TakeLast(6)
                .ToList();
        }
    }
}
