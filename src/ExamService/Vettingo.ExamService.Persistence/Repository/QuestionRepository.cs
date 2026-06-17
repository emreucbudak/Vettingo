using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.Persistence.Repository
{
    public class QuestionRepository(ExamDbContext context) : IQuestionRepository
    {
        private DbSet<MultipleChoiceQuestion> MultipleChoiceQuestionSet => context.Set<MultipleChoiceQuestion>();
        private DbSet<TrueFalseQuestion> TrueFalseQuestionSet => context.Set<TrueFalseQuestion>();
        private DbSet<ClassicQuestion> ClassicQuestionSet => context.Set<ClassicQuestion>();
        private DbSet<CodeCompletionQuestion> CodeCompletionQuestionSet => context.Set<CodeCompletionQuestion>();

        public async Task AddMultipleChoiceQuestionAsync(MultipleChoiceQuestion question)
        {
            await MultipleChoiceQuestionSet.AddAsync(question);
        }

        public void DeleteMultipleChoiceQuestion(MultipleChoiceQuestion question)
        {
            MultipleChoiceQuestionSet.Remove(question);
        }

        public async Task<MultipleChoiceQuestion?> GetMultipleChoiceQuestionByIdAsync(Guid questionId)
        {
            return await MultipleChoiceQuestionSet
                .Include(question => question.Options)
                .FirstOrDefaultAsync(question => question.Id == questionId);
        }

        public async Task<IEnumerable<MultipleChoiceQuestion>> GetMultipleChoiceQuestionsByExamIdAsync(Guid examId)
        {
            return await MultipleChoiceQuestionSet
                .Include(question => question.Options)
                .Where(question => question.ExamId == examId)
                .OrderBy(question => question.DisplayOrder)
                .ToListAsync();
        }

        public async Task AddTrueFalseQuestionAsync(TrueFalseQuestion question)
        {
            await TrueFalseQuestionSet.AddAsync(question);
        }

        public void DeleteTrueFalseQuestion(TrueFalseQuestion question)
        {
            TrueFalseQuestionSet.Remove(question);
        }

        public async Task<TrueFalseQuestion?> GetTrueFalseQuestionByIdAsync(Guid questionId)
        {
            return await TrueFalseQuestionSet.FirstOrDefaultAsync(question => question.Id == questionId);
        }

        public async Task<IEnumerable<TrueFalseQuestion>> GetTrueFalseQuestionsByExamIdAsync(Guid examId)
        {
            return await TrueFalseQuestionSet
                .Where(question => question.ExamId == examId)
                .OrderBy(question => question.DisplayOrder)
                .ToListAsync();
        }

        public async Task AddClassicQuestionAsync(ClassicQuestion question)
        {
            await ClassicQuestionSet.AddAsync(question);
        }

        public void DeleteClassicQuestion(ClassicQuestion question)
        {
            ClassicQuestionSet.Remove(question);
        }

        public async Task<ClassicQuestion?> GetClassicQuestionByIdAsync(Guid questionId)
        {
            return await ClassicQuestionSet.FirstOrDefaultAsync(question => question.Id == questionId);
        }

        public async Task<IEnumerable<ClassicQuestion>> GetClassicQuestionsByExamIdAsync(Guid examId)
        {
            return await ClassicQuestionSet
                .Where(question => question.ExamId == examId)
                .OrderBy(question => question.DisplayOrder)
                .ToListAsync();
        }

        public async Task AddCodeCompletionQuestionAsync(CodeCompletionQuestion question)
        {
            await CodeCompletionQuestionSet.AddAsync(question);
        }

        public void DeleteCodeCompletionQuestion(CodeCompletionQuestion question)
        {
            CodeCompletionQuestionSet.Remove(question);
        }

        public async Task<CodeCompletionQuestion?> GetCodeCompletionQuestionByIdAsync(Guid questionId)
        {
            return await CodeCompletionQuestionSet.FirstOrDefaultAsync(question => question.Id == questionId);
        }

        public async Task<IEnumerable<CodeCompletionQuestion>> GetCodeCompletionQuestionsByExamIdAsync(Guid examId)
        {
            return await CodeCompletionQuestionSet
                .Where(question => question.ExamId == examId)
                .OrderBy(question => question.DisplayOrder)
                .ToListAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateMultipleChoiceQuestion(MultipleChoiceQuestion question)
        {
            MultipleChoiceQuestionSet.Update(question);
        }

        public void UpdateTrueFalseQuestion(TrueFalseQuestion question)
        {
            TrueFalseQuestionSet.Update(question);
        }

        public void UpdateClassicQuestion(ClassicQuestion question)
        {
            ClassicQuestionSet.Update(question);
        }

        public void UpdateCodeCompletionQuestion(CodeCompletionQuestion question)
        {
            CodeCompletionQuestionSet.Update(question);
        }
    }
}
