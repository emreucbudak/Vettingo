using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.DTOs;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;

namespace Vettingo.InterviewService.Persistence.Repository;

public sealed class InterviewRepository(InterviewDbContext context) : IInterviewRepository
{
    public async Task AddAsync(Interview interview, CancellationToken cancellationToken = default)
    {
        await context.Interviews.AddAsync(interview, cancellationToken);
    }

    public void Update(Interview interview)
    {
        context.Interviews.Update(interview);
    }

    public void Delete(Interview interview)
    {
        context.Interviews.Remove(interview);
    }

    public Task<Interview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Interviews.FirstOrDefaultAsync(interview => interview.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Interview>> GetByCompanyAndDateAsync(
        Guid companyId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await context.Interviews.AsNoTracking()
            .Where(interview => interview.CompanyId == companyId && interview.InterviewDate == date)
            .OrderBy(interview => interview.StartedTime)
            .ThenBy(interview => interview.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CompanyInterviewStatistics> GetCompanyStatisticsAsync(
        Guid companyId, DateOnly today, CancellationToken cancellationToken = default)
    {
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);
        var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-daysSinceMonday);
        var weekEnd = weekStart.AddDays(7);

        var statistics = await context.Interviews.AsNoTracking()
            .Where(interview => interview.CompanyId == companyId)
            .GroupBy(interview => 1)
            .Select(group => new CompanyInterviewStatistics(
                group.Count(),
                group.Count(interview => interview.InterviewDate >= monthStart && interview.InterviewDate < monthEnd),
                group.Count(interview => interview.InterviewDate >= weekStart && interview.InterviewDate < weekEnd),
                group.Count(interview => interview.InterviewDate == today)))
            .SingleOrDefaultAsync(cancellationToken);

        return statistics ?? new CompanyInterviewStatistics(0, 0, 0, 0);
    }
}
