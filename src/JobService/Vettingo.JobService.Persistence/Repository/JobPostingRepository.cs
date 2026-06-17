using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.Persistence.Repository
{
    public class JobPostingRepository(JobDbContext context) : IJobPostingRepository
    {
        private DbSet<JobPosting> JobPostingSet => context.Set<JobPosting>();

        public async Task AddJobPostingAsync(JobPosting jobPosting)
        {
            await JobPostingSet.AddAsync(jobPosting);
        }

        public void DeleteJobPosting(JobPosting jobPosting)
        {
            JobPostingSet.Remove(jobPosting);
        }

        public async Task<IEnumerable<JobPosting>> GetAllJobPostingsAsync()
        {
            return await JobPostingSet
                .OrderByDescending(jobPosting => jobPosting.CreatedAt)
                .ToListAsync();
        }

        public async Task<JobPosting?> GetJobPostingByIdAsync(Guid jobPostingId)
        {
            return await JobPostingSet.FirstOrDefaultAsync(jobPosting => jobPosting.Id == jobPostingId);
        }

        public async Task<IEnumerable<JobPosting>> GetJobPostingsByCompanyIdAsync(Guid companyId)
        {
            return await JobPostingSet
                .Where(jobPosting => jobPosting.CompanyId == companyId)
                .OrderByDescending(jobPosting => jobPosting.CreatedAt)
                .ToListAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateJobPosting(JobPosting jobPosting)
        {
            JobPostingSet.Update(jobPosting);
        }
    }
}
