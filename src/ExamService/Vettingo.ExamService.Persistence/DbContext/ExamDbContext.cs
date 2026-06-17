using Microsoft.EntityFrameworkCore;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.DbContext
{
    public class ExamDbContext(DbContextOptions<ExamDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Exam> Exams { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<MultipleChoiceOption> MultipleChoiceOptions { get; set; }
        public DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }
        public DbSet<ClassicQuestion> ClassicQuestions { get; set; }
        public DbSet<CodeCompletionQuestion> CodeCompletionQuestions { get; set; }
        public DbSet<ExamAttempt> ExamAttempts { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
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

            builder.Entity<Exam>()
                .HasMany(exam => exam.ClassicQuestions)
                .WithOne(question => question.Exam)
                .HasForeignKey(question => question.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exam>()
                .HasMany(exam => exam.CodeCompletionQuestions)
                .WithOne(question => question.Exam)
                .HasForeignKey(question => question.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Exam>()
                .HasMany(exam => exam.Attempts)
                .WithOne(attempt => attempt.Exam)
                .HasForeignKey(attempt => attempt.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MultipleChoiceQuestion>()
                .HasMany(question => question.Options)
                .WithOne(option => option.MultipleChoiceQuestion)
                .HasForeignKey(option => option.MultipleChoiceQuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<MultipleChoiceQuestion>()
                .HasMany(question => question.Answers)
                .WithOne(answer => answer.MultipleChoiceQuestion)
                .HasForeignKey(answer => answer.MultipleChoiceQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MultipleChoiceOption>()
                .HasMany(option => option.Answers)
                .WithOne(answer => answer.MultipleChoiceOption)
                .HasForeignKey(answer => answer.MultipleChoiceOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrueFalseQuestion>()
                .HasMany(question => question.Answers)
                .WithOne(answer => answer.TrueFalseQuestion)
                .HasForeignKey(answer => answer.TrueFalseQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ClassicQuestion>()
                .HasMany(question => question.Answers)
                .WithOne(answer => answer.ClassicQuestion)
                .HasForeignKey(answer => answer.ClassicQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CodeCompletionQuestion>()
                .HasMany(question => question.Answers)
                .WithOne(answer => answer.CodeCompletionQuestion)
                .HasForeignKey(answer => answer.CodeCompletionQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ExamAttempt>()
                .HasMany(attempt => attempt.Answers)
                .WithOne(answer => answer.ExamAttempt)
                .HasForeignKey(answer => answer.ExamAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExamAttempt>()
                .Property(attempt => attempt.Status)
                .HasConversion<string>();

            base.OnModelCreating(builder);
        }
    }
}
