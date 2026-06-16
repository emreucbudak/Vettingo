using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.Persistence.Repository
{
    public class ExamRepository(ExamDbContext context) : IExamRepository
    {
        private DbSet<Exam> ExamSet => context.Set<Exam>();

        public async Task AddExamAsync(Exam exam)
        {
            await ExamSet.AddAsync(exam);
        }

        public void DeleteExam(Exam exam)
        {
            ExamSet.Remove(exam);
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await ExamSet
                .Include(exam => exam.Questions)
                .ThenInclude(question => question.Options)
                .OrderByDescending(exam => exam.CreatedAt)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(Guid examId)
        {
            return await ExamSet
                .Include(exam => exam.Questions)
                .ThenInclude(question => question.Options)
                .FirstOrDefaultAsync(exam => exam.Id == examId);
        }

        public Task<int> SaveChangesAsync()
        {
            return context.SaveChangesAsync();
        }

        public void UpdateExam(Exam exam)
        {
            ExamSet.Update(exam);
        }
    }
}
