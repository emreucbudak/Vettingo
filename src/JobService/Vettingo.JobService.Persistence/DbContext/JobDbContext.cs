using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;

namespace Vettingo.JobService.Persistence.DbContext
{
    public class JobDbContext(DbContextOptions<JobDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<JobPosting> JobPostings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.EmploymentType)
                .HasConversion<string>();

            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.WorkingModel)
                .HasConversion<string>();

            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.ExperienceLevel)
                .HasConversion<string>();

            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.Status)
                .HasConversion<string>();

            base.OnModelCreating(builder);
        }
    }
}
