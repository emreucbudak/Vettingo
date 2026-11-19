using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Persistence.DbContext
{
    public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<CompanyStatistic> CompanyStatistics { get; set; }
        public DbSet<HiringVolume> HiringVolumes { get; set; }
        public DbSet<CandidateSource> CandidateSources { get; set; }
        public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AnalyticsDbContext).Assembly);

            base.OnModelCreating(builder);
        }
    }
}
