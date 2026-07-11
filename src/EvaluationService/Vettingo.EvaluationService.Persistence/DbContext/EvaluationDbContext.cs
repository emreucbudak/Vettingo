using Microsoft.EntityFrameworkCore;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Persistence.DbContext;

public sealed class EvaluationDbContext(DbContextOptions<EvaluationDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<EvaluationEntity> Evaluations => Set<EvaluationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EvaluationEntity>(entity =>
        {
            entity.ToTable("Evaluations");

            entity.HasKey(evaluation => evaluation.Id);
            entity.Property(evaluation => evaluation.SkillName)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(evaluation => evaluation.UserId);
            entity.HasIndex(evaluation => new { evaluation.UserId, evaluation.SkillName });
        });

        base.OnModelCreating(modelBuilder);
    }
}
