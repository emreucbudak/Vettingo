using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

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
                // Preserve the existing database representation when renaming Published to Active.
                .HasConversion(
                    status => status == JobPostingStatus.Active ? "Published" : status.ToString(),
                    value => value == "Published" ? JobPostingStatus.Active : Enum.Parse<JobPostingStatus>(value, false));

            builder.Entity<JobPosting>().Property(posting => posting.ApplicationCount).HasDefaultValue(0);

            base.OnModelCreating(builder);
        }
    }
}
