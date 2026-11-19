using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.Configuration;

public sealed class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder
            .Property(exam => exam.OwnerType)
            .HasConversion<string>();

        builder
            .HasMany(exam => exam.MultipleChoiceQuestions)
            .WithOne(question => question.Exam)
            .HasForeignKey(question => question.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(exam => exam.TrueFalseQuestions)
            .WithOne(question => question.Exam)
            .HasForeignKey(question => question.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
