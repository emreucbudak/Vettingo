using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vettingo.ExamService.Domain.Entities;

namespace Vettingo.ExamService.Persistence.Configuration;

public sealed class JobStageExamConfiguration : IEntityTypeConfiguration<JobStageExam>
{
    public void Configure(EntityTypeBuilder<JobStageExam> builder)
    {
        builder
            .Property(jobStageExam => jobStageExam.ExamCandidateList)
            .IsConcurrencyToken();

        builder
            .HasMany(jobExam => jobExam.Attempts)
            .WithOne(attempt => attempt.JobExam)
            .HasForeignKey(attempt => attempt.JobExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
