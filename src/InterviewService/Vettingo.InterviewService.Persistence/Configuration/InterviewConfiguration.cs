using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.InterviewService.Domain.Entities;

namespace Vettingo.InterviewService.Persistence.Configuration;

public sealed class InterviewConfiguration : IEntityTypeConfiguration<Interview>
{
    public void Configure(EntityTypeBuilder<Interview> builder)
    {
        builder
            .HasIndex(interview => new { interview.CompanyId, interview.InterviewDate, interview.StartedTime });
    }
}
