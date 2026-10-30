using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;

namespace Vettingo.InterviewService.Persistence.Repository;

public sealed class InterviewResultRepository(InterviewDbContext context) : IInterviewResultRepository
{
    public async Task AddAsync(InterviewResult interviewResult, CancellationToken cancellationToken = default)
    {
        await context.InterviewResults.AddAsync(interviewResult, cancellationToken);
    }

    public void Update(InterviewResult interviewResult)
    {
        context.InterviewResults.Update(interviewResult);
    }

    public void Delete(InterviewResult interviewResult)
    {
        context.InterviewResults.Remove(interviewResult);
    }

    public Task<InterviewResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.InterviewResults.FirstOrDefaultAsync(interviewResult => interviewResult.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InterviewResult>> GetAllByCompanyIdAsync(
        Guid companyId, CancellationToken cancellationToken = default)
    {
        return await context.InterviewResults.AsNoTracking()
            .Join(context.Interviews.Where(interview => interview.CompanyId == companyId),
                interviewResult => interviewResult.InterviewId,
                interview => interview.Id,
                (interviewResult, interview) => interviewResult)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterviewResult>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.InterviewResults.AsNoTracking()
            .Where(interviewResult => interviewResult.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
