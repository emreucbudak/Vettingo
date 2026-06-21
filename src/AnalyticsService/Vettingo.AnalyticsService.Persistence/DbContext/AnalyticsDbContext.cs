using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Persistence.DbContext
{
    public class AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<CandidateRecommendationAnalysis> CandidateRecommendationAnalyses { get; set; }
        public DbSet<JobPostingPerformanceAnalysis> JobPostingPerformanceAnalyses { get; set; }
        public DbSet<CandidateCvAnalysis> CandidateCvAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CandidateRecommendationAnalysis>(entity =>
            {
                entity.HasKey(analysis => analysis.Id);

                entity.Property(analysis => analysis.CandidateName)
                    .HasMaxLength(200);

                entity.Property(analysis => analysis.CompatibilityRate)
                    .HasPrecision(5, 2);

                entity.HasIndex(analysis => analysis.CompanyId);
                entity.HasIndex(analysis => analysis.JobPostingId);
                entity.HasIndex(analysis => analysis.CandidateId);
            });

            builder.Entity<JobPostingPerformanceAnalysis>(entity =>
            {
                entity.HasKey(analysis => analysis.Id);

                entity.Property(analysis => analysis.AverageCompatibilityRate).HasPrecision(5, 2);
                entity.Property(analysis => analysis.RecommendationHireRate).HasPrecision(5, 2);
                entity.Property(analysis => analysis.ApplicationToHireRate).HasPrecision(5, 2);
                entity.Property(analysis => analysis.CvViewToMatchRate).HasPrecision(5, 2);
                entity.Property(analysis => analysis.TopTenPercentMatchRate).HasPrecision(5, 2);

                entity.HasIndex(analysis => analysis.CompanyId);
                entity.HasIndex(analysis => analysis.JobPostingId).IsUnique();
            });

            builder.Entity<CandidateCvAnalysis>(entity =>
            {
                entity.HasKey(analysis => analysis.Id);

                entity.Property(analysis => analysis.AverageMatchRate)
                    .HasPrecision(5, 2);

                entity.HasIndex(analysis => analysis.CandidateId);
                entity.HasIndex(analysis => new { analysis.CandidateId, analysis.PeriodStart, analysis.PeriodEnd });
            });

            base.OnModelCreating(builder);
        }
    }
}
