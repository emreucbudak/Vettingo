using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.Persistence.Repository
{
    public class JobApplicationRepository(JobDbContext context) : IJobApplicationRepository
    {
        private DbSet<JobApplication> Applications => context.Set<JobApplication>();

        public async Task AddAsync(JobApplication application) => await Applications.AddAsync(application);

        public void Update(JobApplication application) => Applications.Update(application);

        public Task<bool> ExistsAsync(Guid candidateId, Guid jobPostingId) =>
            Applications.AnyAsync(application =>
                application.CandidateId == candidateId && application.JobPostingId == jobPostingId);

        public Task<JobApplication?> GetByIdAsync(Guid applicationId) =>
            Applications.FirstOrDefaultAsync(application => application.Id == applicationId);

        public async Task<IEnumerable<JobApplication>> GetAllAsync(Guid? candidateId = null, Guid? jobPostingId = null)
        {
            IQueryable<JobApplication> query = Applications.AsNoTracking();

            if (candidateId.HasValue)
            {
                query = query.Where(application => application.CandidateId == candidateId.Value);
            }

            if (jobPostingId.HasValue)
            {
                query = query.Where(application => application.JobPostingId == jobPostingId.Value);
            }

            return await query.OrderByDescending(application => application.AppliedAt).ToListAsync();
        }

        public async Task<ApplicationStatistics> GetStatisticsAsync(Guid companyId, CancellationToken cancellationToken = default)
        {
            var result = await Applications.AsNoTracking()
                .Where(application => context.JobPostings.Any(posting =>
                    posting.Id == application.JobPostingId && posting.CompanyId == companyId))
                .GroupBy(application => 1)
                .Select(group => new ApplicationStatistics(group.Count(),
                    group.Count(application => application.Status != Domain.Enums.ApplicationStatus.Rejected)))
                .SingleOrDefaultAsync(cancellationToken);
            return result ?? new ApplicationStatistics(0, 0);
        }

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
    }
}
