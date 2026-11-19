using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class JobStepsConfiguration : IEntityTypeConfiguration<JobSteps>
{
    public void Configure(EntityTypeBuilder<JobSteps> builder)
    {
        builder
            .Property(step => step.StepType)
            .HasConversion<string>();

        var stepDateConverter = new ValueConverter<DateOnly, DateTime>(
            date => date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            timestamp => DateOnly.FromDateTime(timestamp));

        builder.Property(step => step.StartedDate).HasConversion(stepDateConverter);

        builder.Property(step => step.EndDate).HasConversion(stepDateConverter);

        builder
            .HasOne(step => step.JobPosting)
            .WithMany(jobPosting => jobPosting.JobSteps)
            .HasForeignKey(step => step.JobPostingId);
    }
}
