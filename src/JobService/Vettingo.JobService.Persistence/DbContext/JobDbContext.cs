using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.SeedData;

namespace Vettingo.JobService.Persistence.DbContext
{
    public class JobDbContext(DbContextOptions<JobDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobSteps> JobSteps { get; set; }
        public DbSet<PersonalizedJobPostings> PersonalizedJobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobOffer> JobOffers { get; set; }
        public DbSet<Offers> Offers { get; set; }

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
                .HasConversion<string>();

            builder.Entity<JobApplication>()
                .Property(application => application.Status)
                .HasConversion<string>();
            builder.Entity<JobApplication>().HasOne<JobPosting>().WithMany()
                .HasForeignKey(application => application.JobPostingId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobOffer>().HasOne(offer => offer.JobPosting).WithMany()
                .HasForeignKey(offer => offer.JobPostingId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<JobOffer>().Property(offer => offer.Id).ValueGeneratedNever();

            builder.Entity<Offers>().HasOne<JobPosting>().WithMany()
                .HasForeignKey(offer => offer.JobPostingId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Offers>().HasMany(offer => offer.JobOffers).WithOne()
                .HasForeignKey(jobOffer => jobOffer.OffersId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Offers>().Property(offer => offer.CandidateIds).IsConcurrencyToken();

            builder.Entity<JobSteps>()
                .Property(step => step.StepType)
                .HasConversion<string>();

            var stepDateConverter = new ValueConverter<DateOnly, DateTime>(
                date => date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
                timestamp => DateOnly.FromDateTime(timestamp));
            builder.Entity<JobSteps>().Property(step => step.StartedDate).HasConversion(stepDateConverter);
            builder.Entity<JobSteps>().Property(step => step.EndDate).HasConversion(stepDateConverter);

            builder.Entity<JobSteps>()
                .HasOne(step => step.JobPosting)
                .WithMany(jobPosting => jobPosting.JobSteps)
                .HasForeignKey(step => step.JobPostingId);

            builder.ApplyConfiguration(new CityDataSeedConfiguration());
            builder.Entity<JobPosting>().HasOne(jobPosting => jobPosting.City).WithMany()
                .HasForeignKey(jobPosting => jobPosting.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PersonalizedJobPostings>().HasOne(posting => posting.JobPosting).WithMany()
                .HasForeignKey(posting => posting.JobPostingId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
