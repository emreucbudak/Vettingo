using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.Configuration;

public sealed class MultipleChoiceOptionConfiguration : IEntityTypeConfiguration<MultipleChoiceOption>
{
    public void Configure(EntityTypeBuilder<MultipleChoiceOption> builder)
    {
        builder
            .Property(option => option.Id)
            .ValueGeneratedNever();
    }
}
