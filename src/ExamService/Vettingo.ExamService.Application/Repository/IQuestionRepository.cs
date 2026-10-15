using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Application.Repository
{
    public interface IQuestionRepository
    {
        Task AddMultipleChoiceQuestionAsync(MultipleChoiceQuestion question);
        void UpdateMultipleChoiceQuestion(MultipleChoiceQuestion question);
        void DeleteMultipleChoiceQuestion(MultipleChoiceQuestion question);
        Task<MultipleChoiceQuestion?> GetMultipleChoiceQuestionByIdAsync(Guid questionId);
        Task<IEnumerable<MultipleChoiceQuestion>> GetMultipleChoiceQuestionsByExamIdAsync(Guid examId);

        Task AddTrueFalseQuestionAsync(TrueFalseQuestion question);
        void UpdateTrueFalseQuestion(TrueFalseQuestion question);
        void DeleteTrueFalseQuestion(TrueFalseQuestion question);
        Task<TrueFalseQuestion?> GetTrueFalseQuestionByIdAsync(Guid questionId);
        Task<IEnumerable<TrueFalseQuestion>> GetTrueFalseQuestionsByExamIdAsync(Guid examId);

        Task<int> SaveChangesAsync();
    }
}
