using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Persistence.Configuration;

public sealed class CompanyStatisticConfiguration : IEntityTypeConfiguration<CompanyStatistic>
{
    public void Configure(EntityTypeBuilder<CompanyStatistic> builder)
    {
        builder.HasMany(statistic => statistic.HiringVolumes)
            .WithOne()
            .HasForeignKey(volume => volume.CompanyStatisticId);
        builder.HasOne(statistic => statistic.CandidateSource)
            .WithMany()
            .HasForeignKey(statistic => statistic.CandidateSourceId);
    }
}
