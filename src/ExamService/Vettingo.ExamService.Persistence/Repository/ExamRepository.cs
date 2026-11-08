using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Application.Repository;
using Vettingo.ExamService.Domain.Common;
using Vettingo.ExamService.Domain.Entities;
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

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await EntitySet
                .OrderByDescending(entity => entity.CreatedAt)
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            if (typeof(T) == typeof(Exam))
            {
                var exam = await context.Exams
                    .Include(entity => entity.MultipleChoiceQuestions)
                        .ThenInclude(question => question.Options)
                    .Include(entity => entity.TrueFalseQuestions)
                    .FirstOrDefaultAsync(entity => entity.Id == id);

                return exam as T;
            }

            if (typeof(T) == typeof(JobExam))
            {
                var jobExam = await context.JobExams.Include(entity => entity.Exam)
                    .FirstOrDefaultAsync(entity => entity.Id == id);
                return jobExam as T;
            }

            return await EntitySet
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
    }
}
