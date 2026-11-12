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
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CvConfiguration());
        modelBuilder.Entity<CandidateSkill>().HasOne<Cv>().WithMany()
            .HasForeignKey(candidateSkill => candidateSkill.CvId);

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
