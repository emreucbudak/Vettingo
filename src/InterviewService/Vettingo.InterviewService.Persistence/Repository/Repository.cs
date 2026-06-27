using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Repository;
using Vettingo.InterviewService.Domain.Common;
using Vettingo.InterviewService.Persistence.DbContext;

namespace Vettingo.InterviewService.Persistence.Repository
{
    public class Repository<T>(InterviewDbContext context) : IRepository<T> where T : BaseEntity
    {
        private DbSet<T> EntitySet => context.Set<T>();

        public async Task AddAsync(T entity)
        {
            await EntitySet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            EntitySet.Update(entity);
        }

        public void Delete(T entity)
        {
            EntitySet.Remove(entity);
        }

        public async Task<T?> GetByIdAsync(Guid id, params string[] includeProperties)
        {
            return await ApplyIncludes(EntitySet.AsQueryable(), includeProperties)
                .FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params string[] includeProperties)
        {
            IQueryable<T> query = ApplyIncludes(EntitySet.AsQueryable(), includeProperties);

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await EntitySet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await EntitySet.CountAsync(predicate);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        private static IQueryable<T> ApplyIncludes(IQueryable<T> query, IEnumerable<string> includeProperties)
        {
            foreach (string includeProperty in includeProperties.Where(includeProperty => !string.IsNullOrWhiteSpace(includeProperty)))
            {
                query = query.Include(includeProperty);
            }

            return query;
        }
    }
}
