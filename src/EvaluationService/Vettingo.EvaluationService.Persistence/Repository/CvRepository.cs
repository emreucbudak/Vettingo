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

    public void Update(Cv cv, string? targetRole, int? targetSalary,
        IReadOnlyCollection<Language>? languages,
        IReadOnlyCollection<Education>? educations, IReadOnlyCollection<Project>? projects,
        IReadOnlyCollection<Experience>? experiences)
    {
        context.Cvs.Attach(cv);
        var entry = context.Entry(cv);

        if (targetRole is not null)
        {
            entry.Property(item => item.TargetRole).CurrentValue = targetRole;
        }
        if (targetSalary.HasValue)
        {
            entry.Property(item => item.TargetSalary).CurrentValue = targetSalary.Value;
        }
        if (languages is not null)
        {
            context.RemoveRange(cv.Languages);
            cv.SetLanguages(languages);
        }
        if (educations is not null)
        {
            context.RemoveRange(cv.Educations);
            entry.Collection(item => item.Educations).CurrentValue = new List<Education>();
            foreach (var education in educations)
            {
                cv.AddEducation(education);
            }
        }
        if (projects is not null)
        {
            context.RemoveRange(cv.Projects);
            entry.Collection(item => item.Projects).CurrentValue = new List<Project>();
            foreach (var project in projects)
            {
                cv.AddProject(project);
            }
        }
        if (experiences is not null)
        {
            context.RemoveRange(cv.Experiences);
            entry.Collection(item => item.Experiences).CurrentValue = new List<Experience>();
            foreach (var experience in experiences)
            {
                cv.AddExperience(experience);
            }
        }

        if (targetRole is not null || targetSalary.HasValue || languages is not null ||
            educations is not null || projects is not null || experiences is not null)
        {
            entry.Property(item => item.UpdatedAt).CurrentValue = DateTime.UtcNow;
        }
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
