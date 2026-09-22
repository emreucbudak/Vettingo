using Vettingo.ApplicationService.Domain.Entities;

namespace Vettingo.ApplicationService.Application.Repository
{
    public interface IJobApplicationRepository
    {
        Task AddAsync(JobApplication application);
        void Update(JobApplication application);
        Task<bool> ExistsAsync(Guid candidateId, Guid jobPostingId);
        Task<JobApplication?> GetByIdAsync(Guid applicationId);
        Task<IEnumerable<JobApplication>> GetAllAsync(Guid? candidateId = null, Guid? jobPostingId = null);
        Task<ApplicationStatistics> GetStatisticsAsync(Guid[] jobPostingIds, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
