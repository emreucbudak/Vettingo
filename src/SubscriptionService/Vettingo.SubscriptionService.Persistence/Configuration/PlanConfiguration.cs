using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Persistence.Configuration;

public sealed class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id)
            .ValueGeneratedOnAdd();

        builder.Property(plan => plan.PlanName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(plan => plan.Price)
            .IsRequired();

        builder.Property(plan => plan.PlanType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasMany(plan => plan.PlanProperties)
            .WithOne()
            .HasForeignKey("PlanId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(plan => plan.PlanProperties)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
