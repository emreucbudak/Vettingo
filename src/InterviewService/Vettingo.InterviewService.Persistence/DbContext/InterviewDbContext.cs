using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.Persistence.DbContext
{
    public class InterviewDbContext(DbContextOptions<InterviewDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<InterviewQuestion> InterviewQuestions { get; set; }
        public DbSet<InterviewExam> InterviewExams { get; set; }
        public DbSet<InterviewExamQuestion> InterviewExamQuestions { get; set; }
        public DbSet<InterviewAnswer> InterviewAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<InterviewQuestion>()
                .Property(question => question.QuestionText)
                .HasMaxLength(1000);

            builder.Entity<InterviewExam>()
                .Property(exam => exam.Title)
                .HasMaxLength(200);

            builder.Entity<InterviewExam>()
                .Property(exam => exam.Description)
                .HasMaxLength(1000);

            builder.Entity<InterviewExamQuestion>()
                .HasIndex(examQuestion => new { examQuestion.InterviewExamId, examQuestion.InterviewQuestionId })
                .IsUnique();

            builder.Entity<InterviewExamQuestion>()
                .HasOne(examQuestion => examQuestion.InterviewExam)
                .WithMany(exam => exam.Questions)
                .HasForeignKey(examQuestion => examQuestion.InterviewExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InterviewExamQuestion>()
                .HasOne(examQuestion => examQuestion.InterviewQuestion)
                .WithMany(question => question.ExamQuestions)
                .HasForeignKey(examQuestion => examQuestion.InterviewQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InterviewAnswer>()
                .HasOne(answer => answer.InterviewExam)
                .WithMany(exam => exam.Answers)
                .HasForeignKey(answer => answer.InterviewExamId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}
