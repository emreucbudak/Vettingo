using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
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

        public Task<JobApplication?> GetByCandidateAndJobPostingIdAsync(
            Guid candidateId, Guid jobPostingId, CancellationToken cancellationToken = default) =>
            Applications.AsTracking().SingleOrDefaultAsync(application =>
                application.CandidateId == candidateId && application.JobPostingId == jobPostingId,
                cancellationToken);

        public Task<List<JobApplication>> GetPassedApplicationsByJobPostingIdAsync(
            Guid jobPostingId, CancellationToken cancellationToken = default) =>
            Applications.AsTracking()
                .Where(application => application.JobPostingId == jobPostingId
                    && application.IsCurrentStepPassed
                    && application.Status != ApplicationStatus.Elendi)
                .ToListAsync(cancellationToken);

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
                    group.Count(application => application.Status != Domain.Enums.ApplicationStatus.Elendi
                        && application.Status != Domain.Enums.ApplicationStatus.Kabul)))
                .SingleOrDefaultAsync(cancellationToken);
            return result ?? new ApplicationStatistics(0, 0);
        }

        public async Task<CompanyApplicationStatistics> GetCompanyStatisticsAsync(
            Guid companyId, CancellationToken cancellationToken = default)
        {
            var result = await Applications.AsNoTracking()
                .Where(application => context.JobPostings.Any(posting =>
                    posting.Id == application.JobPostingId && posting.CompanyId == companyId))
                .GroupBy(application => 1)
                .Select(group => new CompanyApplicationStatistics(
                    group.Count(),
                    group.Count(application => application.Status == ApplicationStatus.Sınav),
                    group.Count(application => application.Status == ApplicationStatus.Mülakat),
                    group.Count(application => application.Status == ApplicationStatus.Teklif),
                    group.Count(application => application.Status == ApplicationStatus.Elendi)))
                .SingleOrDefaultAsync(cancellationToken);

            return result ?? new CompanyApplicationStatistics(0, 0, 0, 0, 0);
        }

        public async Task<CandidateApplicationStatistics> GetCandidateStatisticsAsync(Guid candidateId, CancellationToken cancellationToken = default)
        {
            var result = await Applications.AsNoTracking()
                .Where(application => application.CandidateId == candidateId)
                .GroupBy(application => 1)
                .Select(group => new CandidateApplicationStatistics(
                    group.Count(),
                    group.Count(a => a.Status == Domain.Enums.ApplicationStatus.Başvuruldu
                        || a.Status == Domain.Enums.ApplicationStatus.Sınav || a.Status == Domain.Enums.ApplicationStatus.Mülakat),
                    group.Count(a => a.Status == Domain.Enums.ApplicationStatus.Mülakat),
                    group.Count(a => a.Status == Domain.Enums.ApplicationStatus.Teklif
                        || a.Status == Domain.Enums.ApplicationStatus.Elendi
                        || a.Status == Domain.Enums.ApplicationStatus.Kabul)))
                .SingleOrDefaultAsync(cancellationToken);
            return result ?? new CandidateApplicationStatistics(0, 0, 0, 0);
        }

        public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
        public async Task<CandidateDashboardApplicationStatistics> GetCandidateDashboardStatisticsAsync(
            Guid userId, CancellationToken cancellationToken = default)
        {
            var result = await Applications.AsNoTracking()
                .Where(application => application.CandidateId == userId)
                .GroupBy(application => 1)
                .Select(group => new CandidateDashboardApplicationStatistics(
                    group.Count(),
                    group.Count(application => application.Status == Domain.Enums.ApplicationStatus.Başvuruldu
                        || application.Status == Domain.Enums.ApplicationStatus.Sınav
                        || application.Status == Domain.Enums.ApplicationStatus.Mülakat),
                    group.Count(application => application.Status == Domain.Enums.ApplicationStatus.Teklif),
                    group.Count(application => application.Status == Domain.Enums.ApplicationStatus.Elendi)))
                .SingleOrDefaultAsync(cancellationToken);
            return result ?? new CandidateDashboardApplicationStatistics(0, 0, 0, 0);
        }
    }
}
