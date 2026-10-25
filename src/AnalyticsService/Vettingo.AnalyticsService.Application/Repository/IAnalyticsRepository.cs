using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Application.Repository
{
    public interface IAnalyticsRepository
    {
        Task AddCompanyStatisticAsync(CompanyStatistic statistic, CancellationToken cancellationToken = default);
        Task AddJobPostingPerformanceAnalysisAsync(JobPostingPerformanceAnalysis analysis);
        void UpdateJobPostingPerformanceAnalysis(JobPostingPerformanceAnalysis analysis);
        Task<JobPostingPerformanceAnalysis?> GetJobPostingPerformanceAnalysisByJobPostingIdAsync(Guid jobPostingId);
        Task<IEnumerable<JobPostingPerformanceAnalysis>> GetJobPostingPerformanceAnalysesByCompanyIdAsync(Guid companyId);
        Task<CompanyStatistic?> GetCompanyStatisticByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task IncrementCompanyJobPostingCountAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<CandidateSource?> GetCandidateSourceByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
