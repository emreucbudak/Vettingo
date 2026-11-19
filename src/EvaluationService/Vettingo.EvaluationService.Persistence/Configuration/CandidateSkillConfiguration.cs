using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Persistence.Configuration;

public sealed class CandidateSkillConfiguration : IEntityTypeConfiguration<CandidateSkill>
{
    public void Configure(EntityTypeBuilder<CandidateSkill> builder)
    {
        builder.HasOne<Cv>().WithMany(cv => cv.CandidateSkills)
            .HasForeignKey(candidateSkill => candidateSkill.CvId);
    }
}
