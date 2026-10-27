using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.Persistence.SeedData;

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
                .Property(jobPosting => jobPosting.RequirementSkills)
                .HasColumnType("jsonb")
                .HasConversion(
                    skills => JsonSerializer.Serialize(skills, (JsonSerializerOptions?)null),
                    json => JsonSerializer.Deserialize<Dictionary<string, int>>(json, (JsonSerializerOptions?)null)!)
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, int>>(
                    (left, right) => left == right ||
                        (left != null && right != null && left.Count == right.Count &&
                         left.All(skill => right.ContainsKey(skill.Key) && right[skill.Key] == skill.Value)),
                    skills => skills.OrderBy(skill => skill.Key, StringComparer.Ordinal)
                        .Aggregate(0, (hash, skill) => HashCode.Combine(hash, skill.Key, skill.Value)),
                    skills => new Dictionary<string, int>(skills)));

            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.EmploymentType)
                .HasConversion<string>();

            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.WorkingModel)
                .HasConversion<string>();

            builder.Entity<JobPosting>()
                .Property(jobPosting => jobPosting.Status)
                .HasConversion(
                    status => status == JobPostingStatus.Active ? "Published" : status.ToString(),
                    value => value == "Published" ? JobPostingStatus.Active : Enum.Parse<JobPostingStatus>(value, false));

            builder.Entity<JobApplication>().Property(application => application.Status).HasConversion<string>();
            builder.Entity<JobApplication>().HasOne<JobPosting>().WithMany()
                .HasForeignKey(application => application.JobPostingId).OnDelete(DeleteBehavior.Restrict);

            builder.ApplyConfiguration(new CityDataSeedConfiguration());
            builder.Entity<JobPosting>().HasOne(jobPosting => jobPosting.City).WithMany()
                .HasForeignKey(jobPosting => jobPosting.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PersonalizedJobPostings>().HasOne(posting => posting.JobPosting).WithMany()
                .HasForeignKey(posting => posting.JobPostingId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
