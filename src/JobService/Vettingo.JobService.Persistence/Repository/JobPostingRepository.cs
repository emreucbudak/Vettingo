using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
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

        public async Task<IReadOnlyList<JobPosting>> SearchJobPostingsAsync(
            JobPostingSearchCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            IQueryable<JobPosting> query = JobPostingSet
                .AsNoTracking()
                .Where(jobPosting => jobPosting.Status == JobPostingStatus.Published);

            if (!string.IsNullOrWhiteSpace(criteria.Title))
            {
                string normalizedTitle = criteria.Title.Trim().ToLower();
                query = query.Where(jobPosting =>
                    jobPosting.Title.ToLower().Contains(normalizedTitle));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Location))
            {
                string normalizedLocation = criteria.Location.Trim().ToLower();
                query = query.Where(jobPosting =>
                    jobPosting.Location.ToLower().Contains(normalizedLocation));
            }

            if (criteria.EmploymentType.HasValue)
            {
                query = query.Where(jobPosting =>
                    jobPosting.EmploymentType == criteria.EmploymentType.Value);
            }

            if (criteria.WorkingModel.HasValue)
            {
                query = query.Where(jobPosting =>
                    jobPosting.WorkingModel == criteria.WorkingModel.Value);
            }

            if (criteria.ExperienceLevel.HasValue)
            {
                query = query.Where(jobPosting =>
                    jobPosting.ExperienceLevel == criteria.ExperienceLevel.Value);
            }

            if (criteria.MinSalary.HasValue)
            {
                query = query.Where(jobPosting =>
                    jobPosting.MaxSalary.HasValue &&
                    jobPosting.MaxSalary.Value >= criteria.MinSalary.Value);
            }

            if (criteria.MaxSalary.HasValue)
            {
                query = query.Where(jobPosting =>
                    jobPosting.MinSalary.HasValue &&
                    jobPosting.MinSalary.Value <= criteria.MaxSalary.Value);
            }

            return await query
                .OrderByDescending(jobPosting => jobPosting.CreatedAt)
                .ToListAsync(cancellationToken);
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
