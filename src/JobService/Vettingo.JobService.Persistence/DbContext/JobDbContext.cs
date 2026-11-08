using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
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
                    status => status == JobPostingStatus.AcceptingApplications ? "Published" : status.ToString(),
                    value => value == "Published" ? JobPostingStatus.AcceptingApplications : Enum.Parse<JobPostingStatus>(value, false));

            builder.Entity<JobApplication>()
                .Property(application => application.Status)
                .HasConversion(status => ToStoredApplicationStatus(status), value => FromStoredApplicationStatus(value));
            builder.Entity<JobApplication>().HasOne<JobPosting>().WithMany()
                .HasForeignKey(application => application.JobPostingId).OnDelete(DeleteBehavior.Restrict);

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

        private static string ToStoredApplicationStatus(ApplicationStatus status) => status switch
        {
            ApplicationStatus.Başvuruldu => "Submitted",
            ApplicationStatus.Sınav => "UnderReview",
            ApplicationStatus.Mülakat => "Interview",
            ApplicationStatus.Teklif => "Offer",
            ApplicationStatus.Elendi => "Rejected",
            ApplicationStatus.İşeAlındı => "Hired",
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Başvuru durumu geçersiz.")
        };

        private static ApplicationStatus FromStoredApplicationStatus(string value) => value.ToUpperInvariant() switch
        {
            "SUBMITTED" => ApplicationStatus.Başvuruldu,
            "UNDERREVIEW" => ApplicationStatus.Sınav,
            "INTERVIEW" => ApplicationStatus.Mülakat,
            "OFFER" => ApplicationStatus.Teklif,
            "REJECTED" => ApplicationStatus.Elendi,
            "HIRED" => ApplicationStatus.İşeAlındı,
            _ => Enum.Parse<ApplicationStatus>(value, ignoreCase: true)
        };
    }
}
