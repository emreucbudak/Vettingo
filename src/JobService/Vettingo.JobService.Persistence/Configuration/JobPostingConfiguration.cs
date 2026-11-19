using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class JobPostingConfiguration : IEntityTypeConfiguration<JobPosting>
{
    public void Configure(EntityTypeBuilder<JobPosting> builder)
    {
        builder
            .Property(jobPosting => jobPosting.EmploymentType)
            .HasConversion<string>();

        builder
            .Property(jobPosting => jobPosting.WorkingModel)
            .HasConversion<string>();

        builder.HasOne(jobPosting => jobPosting.City).WithMany()
            .HasForeignKey(jobPosting => jobPosting.CityId).OnDelete(DeleteBehavior.Restrict);
    }
}
