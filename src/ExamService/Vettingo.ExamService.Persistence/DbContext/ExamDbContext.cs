using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Entities;
using Vettingo.ExamService.Persistence.Configuration;

namespace Vettingo.ExamService.Persistence.DbContext
{
    public class ExamDbContext(DbContextOptions<ExamDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<JobStageExam> JobExams { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<MultipleChoiceOption> MultipleChoiceOptions { get; set; }
        public DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }
        public DbSet<TrueFalseOption> TrueFalseOptions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MultipleChoiceQuestionConfiguration());
            builder.ApplyConfiguration(new TrueFalseQuestionConfiguration());
            builder.ApplyConfiguration(new MultipleChoiceOptionConfiguration());
            builder.ApplyConfiguration(new TrueFalseOptionConfiguration());
            builder.ApplyConfiguration(new ExamConfiguration());
            builder.ApplyConfiguration(new JobStageExamConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
