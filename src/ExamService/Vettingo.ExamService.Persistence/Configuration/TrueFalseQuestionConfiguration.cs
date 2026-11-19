using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.Configuration;

public sealed class TrueFalseQuestionConfiguration : IEntityTypeConfiguration<TrueFalseQuestion>
{
    public void Configure(EntityTypeBuilder<TrueFalseQuestion> builder)
    {
        builder
            .Property(question => question.Id)
            .ValueGeneratedNever();

        builder
            .HasMany(question => question.Options)
            .WithOne(option => option.TrueFalseQuestion)
            .HasForeignKey(option => option.TrueFalseQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
