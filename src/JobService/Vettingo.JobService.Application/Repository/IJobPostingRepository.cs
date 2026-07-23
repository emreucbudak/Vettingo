using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Application.Repository
{
    public interface IJobPostingRepository
    {
        Task AddJobPostingAsync(JobPosting jobPosting);
        void UpdateJobPosting(JobPosting jobPosting);
        void DeleteJobPosting(JobPosting jobPosting);
        Task<JobPosting?> GetJobPostingByIdAsync(Guid jobPostingId);
        Task<IEnumerable<JobPosting>> GetAllJobPostingsAsync();
        Task<IEnumerable<JobPosting>> GetJobPostingsByCompanyIdAsync(Guid companyId);
        Task<IReadOnlyList<JobPosting>> SearchJobPostingsAsync(
            JobPostingSearchCriteria criteria,
            CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync();
    }
}
