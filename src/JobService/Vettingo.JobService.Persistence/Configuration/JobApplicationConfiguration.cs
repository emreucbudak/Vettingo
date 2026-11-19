using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder
            .Property(application => application.Status)
            .HasConversion<string>();

        builder.HasOne<JobPosting>().WithMany()
            .HasForeignKey(application => application.JobPostingId).OnDelete(DeleteBehavior.Restrict);
    }
}
