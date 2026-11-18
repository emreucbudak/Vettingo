using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Application.Repository
{
    public interface IExamRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetByIdAsync(Guid id);
        Task<Exam?> GetExamByIdAndCompanyIdAsync(Guid examId, Guid companyId, CancellationToken cancellationToken);
        Task<Exam?> GetExamDetailsByIdAndCompanyIdAsync(Guid examId, Guid companyId, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IReadOnlyList<Exam>> GetExamsByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
        Task<JobStageExam?> GetJobStageExamByJobIdAndExamIdAsync(
            Guid jobId, Guid examId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}
