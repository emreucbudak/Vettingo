using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Persistence.DbContext
{
    public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<CompanyStatistic> CompanyStatistics { get; set; }
        public DbSet<HiringVolume> HiringVolumes { get; set; }
        public DbSet<CandidateSource> CandidateSources { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CompanyStatistic>(entity =>
            {
                entity.HasMany(statistic => statistic.HiringVolumes)
                    .WithOne()
                    .HasForeignKey(volume => volume.CompanyStatisticId);
                entity.HasOne(statistic => statistic.CandidateSource)
                    .WithMany()
                    .HasForeignKey(statistic => statistic.CandidateSourceId);
            });

            base.OnModelCreating(builder);
        }
    }
}
