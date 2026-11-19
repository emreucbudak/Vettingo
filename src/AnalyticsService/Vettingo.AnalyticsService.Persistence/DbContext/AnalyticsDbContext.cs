using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.Configuration;

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
            builder.ApplyConfiguration(new ProcessedEventConfiguration());
            builder.ApplyConfiguration(new CompanyStatisticConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
