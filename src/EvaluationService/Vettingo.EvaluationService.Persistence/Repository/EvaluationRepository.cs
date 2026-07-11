using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Persistence.DbContext;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Persistence.Repository;

public sealed class EvaluationRepository(EvaluationDbContext context) : IEvaluationRepository
{
    public async Task AddAsync(EvaluationEntity evaluation, CancellationToken cancellationToken = default)
    {
        await context.Evaluations.AddAsync(evaluation, cancellationToken);
    }

    public void Update(EvaluationEntity evaluation)
    {
        context.Evaluations.Update(evaluation);
    }

    public void Delete(EvaluationEntity evaluation)
    {
        context.Evaluations.Remove(evaluation);
    }

    public Task<EvaluationEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Evaluations.FirstOrDefaultAsync(evaluation => evaluation.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<EvaluationEntity>> GetAllAsync(
        Guid? userId,
        string? skillName,
        CancellationToken cancellationToken = default)
    {
        IQueryable<EvaluationEntity> query = context.Evaluations.AsNoTracking();

        if (userId.HasValue)
        {
            query = query.Where(evaluation => evaluation.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(skillName))
        {
            string normalizedSkillName = skillName.Trim();
            query = query.Where(evaluation => evaluation.SkillName == normalizedSkillName);
        }

        return await query
            .OrderByDescending(evaluation => evaluation.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
