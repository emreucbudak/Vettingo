using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.Configuration;

namespace Vettingo.InterviewService.Persistence.DbContext
{
    public class InterviewDbContext(DbContextOptions<InterviewDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<InterviewResult> InterviewResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new InterviewConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
