using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.Configuration;

public sealed class TrueFalseOptionConfiguration : IEntityTypeConfiguration<TrueFalseOption>
{
    public void Configure(EntityTypeBuilder<TrueFalseOption> builder)
    {
        builder
            .Property(option => option.Id)
            .ValueGeneratedNever();
    }
}
