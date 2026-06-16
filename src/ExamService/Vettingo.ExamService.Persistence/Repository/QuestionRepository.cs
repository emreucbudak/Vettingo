using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.Persistence.Repository
{
    public class QuestionRepository(ExamDbContext context) : IQuestionRepository
    {
        private DbSet<Question> QuestionSet => context.Set<Question>();

        public async Task AddQuestionAsync(Question question)
        {
            await QuestionSet.AddAsync(question);
        }

        public void DeleteQuestion(Question question)
        {
            QuestionSet.Remove(question);
        }

        public async Task<Question?> GetQuestionByIdAsync(Guid questionId)
        {
            return await QuestionSet
                .Include(question => question.Options)
                .FirstOrDefaultAsync(question => question.Id == questionId);
        }

        public async Task<IEnumerable<Question>> GetQuestionsByExamIdAsync(Guid examId)
        {
            return await QuestionSet
                .Include(question => question.Options)
                .Where(question => question.ExamId == examId)
                .OrderBy(question => question.DisplayOrder)
                .ToListAsync();
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateQuestion(Question question)
        {
            QuestionSet.Update(question);
        }
    }
}
