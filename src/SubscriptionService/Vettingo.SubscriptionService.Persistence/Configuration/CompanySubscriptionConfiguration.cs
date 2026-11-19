using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Persistence.Configuration;

public sealed class CompanySubscriptionConfiguration : IEntityTypeConfiguration<CompanySubscription>
{
    public void Configure(EntityTypeBuilder<CompanySubscription> builder)
    {
        builder.ToTable("CompanySubscriptions");
        builder.HasKey(subscription => subscription.Id);

        builder.Property(subscription => subscription.Id)
            .ValueGeneratedNever();

        builder.Property(subscription => subscription.StartDate)
            .IsRequired();

        builder.HasOne(subscription => subscription.Plan)
            .WithMany()
            .HasForeignKey(subscription => subscription.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(subscription => subscription.CompanyId);
        builder.HasIndex(subscription => subscription.PlanId);
    }
}
