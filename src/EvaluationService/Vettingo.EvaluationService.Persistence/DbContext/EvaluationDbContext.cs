using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Domain.Models;
using Vettingo.EvaluationService.Persistence.Configuration;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Persistence.DbContext;

public sealed class EvaluationDbContext(DbContextOptions<EvaluationDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<EvaluationEntity> Evaluations => Set<EvaluationEntity>();
    public DbSet<Cv> Cvs => Set<Cv>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CvConfiguration());
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
