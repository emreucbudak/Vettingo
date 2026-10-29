using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Application.Repository
{
    public interface IAnalyticsRepository
    {
        Task AddCompanyStatisticAsync(CompanyStatistic statistic, CancellationToken cancellationToken = default);
        Task<CompanyStatistic?> GetCompanyStatisticByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<int> IncrementCompanyJobPostingCountAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<CandidateSource?> GetCandidateSourceByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
