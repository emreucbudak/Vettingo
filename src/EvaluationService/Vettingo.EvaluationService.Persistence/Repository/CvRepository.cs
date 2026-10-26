using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Persistence.DbContext;

namespace Vettingo.EvaluationService.Persistence.Repository;

public sealed class CvRepository(EvaluationDbContext context) : ICvRepository
{
    public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Cvs.AsNoTracking().AnyAsync(cv => cv.UserId == userId, cancellationToken);
    }
}
