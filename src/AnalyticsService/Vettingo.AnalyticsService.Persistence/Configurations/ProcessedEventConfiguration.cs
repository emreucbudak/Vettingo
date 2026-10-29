using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.AnalyticsService.Domain.Entities;

namespace Vettingo.AnalyticsService.Persistence.Configurations;

public sealed class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.HasKey(x => x.EventId);
        builder.Property(x => x.EventId).ValueGeneratedNever();
    }
}
