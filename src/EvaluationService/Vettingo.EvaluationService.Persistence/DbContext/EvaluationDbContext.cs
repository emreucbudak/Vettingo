using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Domain.Models;
using Vettingo.EvaluationService.Persistence.Configuration;

namespace Vettingo.EvaluationService.Persistence.DbContext;

public sealed class EvaluationDbContext(DbContextOptions<EvaluationDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<Cv> Cvs => Set<Cv>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CvConfiguration());
        modelBuilder.ApplyConfiguration(new CandidateSkillConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
