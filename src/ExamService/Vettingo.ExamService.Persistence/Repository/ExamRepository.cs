using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.Persistence.Repository
{
    public class ExamRepository<T>(ExamDbContext context) : IExamRepository<T> where T : BaseEntity
    {
        private DbSet<T> EntitySet => context.Set<T>();

        public async Task AddAsync(T entity)
        {
            await EntitySet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            EntitySet.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(params string[] includes)
        {
            return await IncludeRelations(includes)
                .OrderByDescending(entity => entity.CreatedAt)
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id, params string[] includes)
        {
            return await IncludeRelations(includes)
                .FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            EntitySet.Update(entity);
        }

        private IQueryable<T> IncludeRelations(string[] includes)
        {
            IQueryable<T> query = EntitySet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}
