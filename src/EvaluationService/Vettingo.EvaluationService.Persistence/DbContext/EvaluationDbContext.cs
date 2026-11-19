using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.Persistence.DbContext;

public sealed class EvaluationDbContext(DbContextOptions<EvaluationDbContext> options)
    : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<Cv> Cvs => Set<Cv>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CandidateSkill> CandidateSkills => Set<CandidateSkill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EvaluationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
