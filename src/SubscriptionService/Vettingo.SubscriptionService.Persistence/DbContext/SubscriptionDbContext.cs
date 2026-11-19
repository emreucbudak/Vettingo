using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.Configuration;

namespace Vettingo.SubscriptionService.Persistence.DbContext
{
    public class SubscriptionDbContext(DbContextOptions<SubscriptionDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Plan> Plans { get; set; }
        public DbSet<CompanySubscription> CompanySubscriptions { get; set; }
        public DbSet<CandidateSubscription> CandidateSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PlanConfiguration());
            builder.ApplyConfiguration(new PlanPropertiesConfiguration());
            builder.ApplyConfiguration(new CompanySubscriptionConfiguration());
            builder.ApplyConfiguration(new CandidateSubscriptionConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
