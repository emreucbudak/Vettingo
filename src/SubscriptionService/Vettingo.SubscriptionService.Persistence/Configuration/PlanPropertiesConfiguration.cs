using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Persistence.Configuration;

public sealed class PlanPropertiesConfiguration : IEntityTypeConfiguration<PlanProperties>
{
    public void Configure(EntityTypeBuilder<PlanProperties> builder)
    {
        builder.HasKey(planProperties => planProperties.Id);

        builder.Property(planProperties => planProperties.Id)
            .ValueGeneratedOnAdd();

        builder.Property(planProperties => planProperties.PropertiesName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(planProperties => planProperties.Count)
            .IsRequired();
    }
}
