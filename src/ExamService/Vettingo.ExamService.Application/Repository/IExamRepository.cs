using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Application.Repository
{
    public interface IExamRepository
    {
        Task AddExamAsync(Exam exam);
        void UpdateExam(Exam exam);
        void DeleteExam(Exam exam);
        Task<Exam?> GetExamByIdAsync(Guid examId);
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<int> SaveChangesAsync();
    }
}
