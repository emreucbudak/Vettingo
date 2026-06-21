using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Application.Repository
{
    public interface IAnalyticsRepository
    {
        Task AddCandidateRecommendationAnalysisAsync(CandidateRecommendationAnalysis analysis);
        Task<IEnumerable<CandidateRecommendationAnalysis>> GetCandidateRecommendationAnalysesByCompanyIdAsync(Guid companyId);
        Task<IEnumerable<CandidateRecommendationAnalysis>> GetCandidateRecommendationAnalysesByJobPostingIdAsync(Guid jobPostingId);
        Task AddJobPostingPerformanceAnalysisAsync(JobPostingPerformanceAnalysis analysis);
        void UpdateJobPostingPerformanceAnalysis(JobPostingPerformanceAnalysis analysis);
        Task<JobPostingPerformanceAnalysis?> GetJobPostingPerformanceAnalysisByJobPostingIdAsync(Guid jobPostingId);
        Task<IEnumerable<JobPostingPerformanceAnalysis>> GetJobPostingPerformanceAnalysesByCompanyIdAsync(Guid companyId);
        Task AddCandidateCvAnalysisAsync(CandidateCvAnalysis analysis);
        Task<IEnumerable<CandidateCvAnalysis>> GetCandidateCvAnalysesByCandidateIdAsync(Guid candidateId);
        Task<CandidateCvAnalysis?> GetLatestCandidateCvAnalysisAsync(Guid candidateId);
        Task<int> SaveChangesAsync();
    }
}
