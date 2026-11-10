using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.DbContext
{
    public class ExamDbContext(DbContextOptions<ExamDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<JobStageExam> JobExams { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<MultipleChoiceOption> MultipleChoiceOptions { get; set; }
        public DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MultipleChoiceQuestion>()
                .Property(question => question.Id)
                .ValueGeneratedNever();

            builder.Entity<TrueFalseQuestion>()
                .Property(question => question.Id)
                .ValueGeneratedNever();

            builder.Entity<MultipleChoiceOption>()
                .Property(option => option.Id)
                .ValueGeneratedNever();

            builder.Entity<Exam>()
                .Property(exam => exam.OwnerType)
                .HasConversion<string>();

            builder.Entity<Exam>()
                .HasMany(exam => exam.MultipleChoiceQuestions)
                .WithOne(question => question.Exam)
                .HasForeignKey(question => question.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exam>()
                .HasMany(exam => exam.TrueFalseQuestions)
                .WithOne(question => question.Exam)
                .HasForeignKey(question => question.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobStageExam>()
                .HasMany(jobExam => jobExam.Attempts)
                .WithOne(attempt => attempt.JobExam)
                .HasForeignKey(attempt => attempt.JobExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MultipleChoiceQuestion>()
                .HasMany(question => question.Options)
                .WithOne(option => option.MultipleChoiceQuestion)
                .HasForeignKey(option => option.MultipleChoiceQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}
