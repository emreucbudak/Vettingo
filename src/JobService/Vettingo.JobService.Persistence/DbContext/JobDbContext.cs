using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Persistence.DbContext
{
    public class JobDbContext(DbContextOptions<JobDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<PersonalizedJobPostings> PersonalizedJobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

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

            builder.Entity<JobApplication>().Property(application => application.Status).HasConversion<string>();
            builder.Entity<JobApplication>().HasIndex(application => application.AppliedAt);
            builder.Entity<JobApplication>().HasOne<JobPosting>().WithMany()
                .HasForeignKey(application => application.JobPostingId).OnDelete(DeleteBehavior.Restrict);

            builder.ApplyConfiguration(new CityDataSeedConfiguration());
            builder.Entity<JobPosting>().HasOne(jobPosting => jobPosting.City).WithMany()
                .HasForeignKey(jobPosting => jobPosting.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PersonalizedJobPostings>().HasOne(jobPosting => jobPosting.City).WithMany()
                .HasForeignKey(jobPosting => jobPosting.CityId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
