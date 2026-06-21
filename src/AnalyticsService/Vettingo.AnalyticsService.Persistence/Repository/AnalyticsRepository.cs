using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;

namespace Vettingo.AnalyticsService.Persistence.Repository
{
    public class AnalyticsRepository(AnalyticsDbContext context) : IAnalyticsRepository
    {
        private DbSet<CandidateRecommendationAnalysis> CandidateRecommendationAnalysisSet => context.Set<CandidateRecommendationAnalysis>();
        private DbSet<JobPostingPerformanceAnalysis> JobPostingPerformanceAnalysisSet => context.Set<JobPostingPerformanceAnalysis>();
        private DbSet<CandidateCvAnalysis> CandidateCvAnalysisSet => context.Set<CandidateCvAnalysis>();

        public async Task AddCandidateRecommendationAnalysisAsync(CandidateRecommendationAnalysis analysis)
        {
            await CandidateRecommendationAnalysisSet.AddAsync(analysis);
        }

        public async Task<IEnumerable<CandidateRecommendationAnalysis>> GetCandidateRecommendationAnalysesByCompanyIdAsync(Guid companyId)
        {
            return await CandidateRecommendationAnalysisSet
                .Where(analysis => analysis.CompanyId == companyId)
                .OrderByDescending(analysis => analysis.RecommendedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CandidateRecommendationAnalysis>> GetCandidateRecommendationAnalysesByJobPostingIdAsync(Guid jobPostingId)
        {
            return await CandidateRecommendationAnalysisSet
                .Where(analysis => analysis.JobPostingId == jobPostingId)
                .OrderByDescending(analysis => analysis.RecommendedAt)
                .ToListAsync();
        }

        public async Task AddJobPostingPerformanceAnalysisAsync(JobPostingPerformanceAnalysis analysis)
        {
            await JobPostingPerformanceAnalysisSet.AddAsync(analysis);
        }

        public async Task<JobPostingPerformanceAnalysis?> GetJobPostingPerformanceAnalysisByJobPostingIdAsync(Guid jobPostingId)
        {
            return await JobPostingPerformanceAnalysisSet.FirstOrDefaultAsync(analysis => analysis.JobPostingId == jobPostingId);
        }

        public async Task<IEnumerable<JobPostingPerformanceAnalysis>> GetJobPostingPerformanceAnalysesByCompanyIdAsync(Guid companyId)
        {
            return await JobPostingPerformanceAnalysisSet
                .Where(analysis => analysis.CompanyId == companyId)
                .OrderByDescending(analysis => analysis.UpdatedAt ?? analysis.CreatedAt)
                .ToListAsync();
        }

        public void UpdateJobPostingPerformanceAnalysis(JobPostingPerformanceAnalysis analysis)
        {
            JobPostingPerformanceAnalysisSet.Update(analysis);
        }

        public async Task AddCandidateCvAnalysisAsync(CandidateCvAnalysis analysis)
        {
            await CandidateCvAnalysisSet.AddAsync(analysis);
        }

        public async Task<IEnumerable<CandidateCvAnalysis>> GetCandidateCvAnalysesByCandidateIdAsync(Guid candidateId)
        {
            return await CandidateCvAnalysisSet
                .Where(analysis => analysis.CandidateId == candidateId)
                .OrderByDescending(analysis => analysis.PeriodEnd)
                .ToListAsync();
        }

        public async Task<CandidateCvAnalysis?> GetLatestCandidateCvAnalysisAsync(Guid candidateId)
        {
            return await CandidateCvAnalysisSet
                .Where(analysis => analysis.CandidateId == candidateId)
                .OrderByDescending(analysis => analysis.PeriodEnd)
                .FirstOrDefaultAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }
    }
}
