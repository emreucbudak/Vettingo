using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class PersonalizedJobPostingsConfiguration : IEntityTypeConfiguration<PersonalizedJobPostings>
{
    public void Configure(EntityTypeBuilder<PersonalizedJobPostings> builder)
    {
        builder.HasOne(posting => posting.JobPosting).WithMany()
            .HasForeignKey(posting => posting.JobPostingId).OnDelete(DeleteBehavior.Restrict);
    }
}
