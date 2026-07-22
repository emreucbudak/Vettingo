using Microsoft.EntityFrameworkCore;
using Vettingo.ApplicationService.Domain.Entities;

namespace Vettingo.ApplicationService.Persistence.DbContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<JobApplication>()
                .Property(application => application.Status)
                .HasConversion<string>();

            builder.Entity<JobApplication>()
                .HasIndex(application => new { application.CandidateId, application.JobPostingId })
                .IsUnique();

            builder.Entity<JobApplication>()
                .HasIndex(application => application.AppliedAt);

            base.OnModelCreating(builder);
        }
    }
}
