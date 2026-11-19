using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.Configuration;

public sealed class CandidateSkillConfiguration : IEntityTypeConfiguration<CandidateSkill>
{
    public void Configure(EntityTypeBuilder<CandidateSkill> builder)
    {
        builder.HasOne<JobPosting>().WithMany(posting => posting.CandidateSkills)
            .HasForeignKey(candidateSkill => candidateSkill.JobPostingId);
    }
}
