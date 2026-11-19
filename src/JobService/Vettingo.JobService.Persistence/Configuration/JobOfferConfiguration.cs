using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class JobOfferConfiguration : IEntityTypeConfiguration<JobOffer>
{
    public void Configure(EntityTypeBuilder<JobOffer> builder)
    {
        builder.HasOne(offer => offer.JobPosting).WithMany()
            .HasForeignKey(offer => offer.JobPostingId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(offer => offer.Id).ValueGeneratedNever();
    }
}
