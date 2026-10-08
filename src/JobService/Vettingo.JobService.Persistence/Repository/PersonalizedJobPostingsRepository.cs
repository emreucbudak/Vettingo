using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Repository;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.Persistence.Repository;

public class PersonalizedJobPostingsRepository(JobDbContext context) : IPersonalizedJobPostingsRepository
{
    public async Task AddAsync(PersonalizedJobPostings posting, CancellationToken cancellationToken = default) =>
        await context.PersonalizedJobPostings.AddAsync(posting, cancellationToken);

    public void Delete(PersonalizedJobPostings posting) => context.PersonalizedJobPostings.Remove(posting);

    public Task<PersonalizedJobPostings?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.PersonalizedJobPostings.FirstOrDefaultAsync(posting => posting.Id == id, cancellationToken);

    public async Task<IEnumerable<PersonalizedJobPostings>> GetAllAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var query = context.PersonalizedJobPostings.AsNoTracking().Include(posting => posting.City).AsQueryable();
        if (userId.HasValue)
        {
            query = query.Where(posting => posting.UserId == userId.Value);
        }

        return await query.OrderByDescending(posting => posting.PublishedDate)
            .ThenByDescending(posting => posting.CreatedAt).ToListAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<IEnumerable<PersonalizedJobPostings>> GetLatestAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await context.PersonalizedJobPostings.AsNoTracking()
            .Include(posting => posting.City)
            .Where(posting => posting.UserId == userId)
            .OrderByDescending(posting => posting.PublishedDate)
            .ThenByDescending(posting => posting.CreatedAt)
            .ThenByDescending(posting => posting.Id)
            .Take(3)
            .ToListAsync(cancellationToken);
}
