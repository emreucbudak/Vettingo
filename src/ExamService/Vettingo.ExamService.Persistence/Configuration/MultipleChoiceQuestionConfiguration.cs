using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.Configuration;

public sealed class MultipleChoiceQuestionConfiguration : IEntityTypeConfiguration<MultipleChoiceQuestion>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceQuestion> builder)
    {
        builder
            .Property(question => question.Id)
            .ValueGeneratedNever();

        builder
            .HasMany(question => question.Options)
            .WithOne(option => option.MultipleChoiceQuestion)
            .HasForeignKey(option => option.MultipleChoiceQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
