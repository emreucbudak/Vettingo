using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class OffersConfiguration : IEntityTypeConfiguration<Offers>
{
    public void Configure(EntityTypeBuilder<Offers> builder)
    {
        builder.HasOne<JobPosting>().WithMany()
            .HasForeignKey(offer => offer.JobPostingId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(offer => offer.JobOffers).WithOne()
            .HasForeignKey(jobOffer => jobOffer.OffersId).OnDelete(DeleteBehavior.Restrict);

        builder.Property(offer => offer.CandidateIds).IsConcurrencyToken();
    }
}
