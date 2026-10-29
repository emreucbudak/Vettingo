using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;

namespace Vettingo.AnalyticsService.Persistence.Repository
{
    public class AnalyticsRepository(AnalyticsDbContext context) : IAnalyticsRepository
    {
        private DbSet<CompanyStatistic> CompanyStatisticSet => context.Set<CompanyStatistic>();

        public async Task AddCompanyStatisticAsync(CompanyStatistic statistic, CancellationToken cancellationToken = default)
        {
            await CompanyStatisticSet.AddAsync(statistic, cancellationToken);
        }

        public async Task<CompanyStatistic?> GetCompanyStatisticByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await CompanyStatisticSet
                .AsNoTracking()
                .Include(statistic => statistic.HiringVolumes)
                .FirstOrDefaultAsync(statistic => statistic.CompanyId == companyId, cancellationToken);
        }

        public Task<int> IncrementCompanyJobPostingCountAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var updatedAt = DateTime.UtcNow;

            return CompanyStatisticSet
                .Where(statistic => statistic.CompanyId == companyId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(statistic => statistic.TotalJobPostingCount, statistic => statistic.TotalJobPostingCount + 1)
                    .SetProperty(statistic => statistic.ActiveJobPostingCount, statistic => statistic.ActiveJobPostingCount + 1)
                    .SetProperty(statistic => statistic.UpdatedAt, updatedAt),
                    cancellationToken);
        }

        public async Task<CandidateSource?> GetCandidateSourceByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            return await CompanyStatisticSet
                .AsNoTracking()
                .Where(statistic => statistic.CompanyId == companyId)
                .Select(statistic => statistic.CandidateSource)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
