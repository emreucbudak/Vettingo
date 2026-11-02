using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Domain.Models;
using Vettingo.EvaluationService.Persistence.DbContext;

namespace Vettingo.EvaluationService.Persistence.Repository;

public sealed class CvRepository(EvaluationDbContext context) : ICvRepository
{
    public async Task AddAsync(Cv cv, CancellationToken cancellationToken = default)
    {
        await context.Cvs.AddAsync(cv, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Cvs.AsNoTracking().AnyAsync(cv => cv.UserId == userId, cancellationToken);
    }

    public Task<Cv?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Cvs
            .AsNoTracking()
            .Include(cv => cv.Educations)
            .Include(cv => cv.Projects)
            .Include(cv => cv.Experiences)
            .AsSplitQuery()
            .SingleOrDefaultAsync(cv => cv.UserId == userId, cancellationToken);
    }
}
