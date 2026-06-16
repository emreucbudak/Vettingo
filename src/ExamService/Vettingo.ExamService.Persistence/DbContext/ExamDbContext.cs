using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.DbContext
{
    public class ExamDbContext(DbContextOptions<ExamDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Exam>()
                .HasMany(exam => exam.Questions)
                .WithOne(question => question.Exam)
                .HasForeignKey(question => question.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exam>()
                .HasMany(exam => exam.Attempts)
                .WithOne(attempt => attempt.Exam)
                .HasForeignKey(attempt => attempt.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasMany(question => question.Options)
                .WithOne(option => option.Question)
                .HasForeignKey(option => option.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasMany(question => question.Answers)
                .WithOne(answer => answer.Question)
                .HasForeignKey(answer => answer.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuestionOption>()
                .HasMany(option => option.Answers)
                .WithOne(answer => answer.QuestionOption)
                .HasForeignKey(answer => answer.QuestionOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamAttempt>()
                .HasMany(attempt => attempt.Answers)
                .WithOne(answer => answer.ExamAttempt)
                .HasForeignKey(answer => answer.ExamAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .Property(question => question.QuestionType)
                .HasConversion<string>();

            builder.Entity<ExamAttempt>()
                .Property(attempt => attempt.Status)
                .HasConversion<string>();

            base.OnModelCreating(builder);
        }
    }
}
