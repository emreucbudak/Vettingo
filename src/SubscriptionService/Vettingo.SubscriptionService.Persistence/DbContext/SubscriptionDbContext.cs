using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.Persistence.DbContext
{
    public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanProperties> PlanProperties { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Plan>(entity =>
            {
                entity.HasKey(plan => plan.Id);

                entity.Property(plan => plan.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(plan => plan.PlanName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(plan => plan.Price)
                    .IsRequired();

                entity.HasMany(plan => plan.PlanProperties)
                    .WithOne()
                    .HasForeignKey("PlanId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PlanProperties>(entity =>
            {
                entity.HasKey(planProperties => planProperties.Id);

                entity.Property(planProperties => planProperties.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(planProperties => planProperties.PropertiesName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(planProperties => planProperties.Count)
                    .IsRequired();
            });

            builder.Entity<Subscription>(entity =>
            {
                entity.HasKey(subscription => subscription.Id);

                entity.Property(subscription => subscription.Id)
                    .ValueGeneratedNever();

                entity.Property(subscription => subscription.StartDate)
                    .IsRequired();

                entity.HasOne(subscription => subscription.Plan)
                    .WithMany()
                    .HasForeignKey(subscription => subscription.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(subscription => subscription.CompanyId);
                entity.HasIndex(subscription => subscription.PlanId);
            });

            base.OnModelCreating(builder);
        }
    }
}
