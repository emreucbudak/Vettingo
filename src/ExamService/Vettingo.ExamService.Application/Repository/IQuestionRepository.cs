using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Application.Repository
{
    public interface IQuestionRepository
    {
        Task AddQuestionAsync(Question question);
        void UpdateQuestion(Question question);
        void DeleteQuestion(Question question);
        Task<Question?> GetQuestionByIdAsync(Guid questionId);
        Task<IEnumerable<Question>> GetQuestionsByExamIdAsync(Guid examId);
        Task<int> SaveChangesAsync();
    }
}
