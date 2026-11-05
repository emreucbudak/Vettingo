using Vettingo.ExamService.Domain.Common;

namespace Vettingo.ExamService.Application.Repository
{
    public interface IExamRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetByIdAsync(Guid id, params string[] includes);
        Task<IEnumerable<T>> GetAllAsync(params string[] includes);
        Task<int> SaveChangesAsync();
    }
}
