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

        public async Task<IReadOnlyList<Exam>> GetExamsByCompanyIdAsync(
            Guid companyId, CancellationToken cancellationToken)
        {
            return await context.Exams
                .AsNoTracking()
                .Where(exam => exam.CompanyId == companyId)
                .Include(exam => exam.MultipleChoiceQuestions)
                .Include(exam => exam.TrueFalseQuestions)
                .ToListAsync(cancellationToken);
        }

        public Task<Exam?> GetExamByIdAndCompanyIdAsync(Guid examId, Guid companyId, CancellationToken cancellationToken)
        {
            return context.Exams.FirstOrDefaultAsync(
                exam => exam.Id == examId && exam.CompanyId == companyId, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            if (typeof(T) == typeof(Exam))
            {
                var exam = await context.Exams
                    .Include(entity => entity.MultipleChoiceQuestions)
                        .ThenInclude(question => question.Options)
                    .Include(entity => entity.TrueFalseQuestions)
                        .ThenInclude(question => question.Options)
                    .FirstOrDefaultAsync(entity => entity.Id == id);

                return exam as T;
            }

            if (typeof(T) == typeof(JobStageExam))
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
