using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Application.Repository
{
    public interface IJobApplicationRepository
    {
        Task AddAsync(JobApplication application);
        void Update(JobApplication application);
        Task<bool> ExistsAsync(Guid candidateId, Guid jobPostingId);
        Task<JobApplication?> GetByIdAsync(Guid applicationId);
        Task<IEnumerable<JobApplication>> GetAllAsync(Guid? candidateId = null, Guid? jobPostingId = null);
        Task<ApplicationStatistics> GetStatisticsAsync(Guid companyId, CancellationToken cancellationToken = default);
        Task<CandidateApplicationStatistics> GetCandidateStatisticsAsync(Guid candidateId, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
