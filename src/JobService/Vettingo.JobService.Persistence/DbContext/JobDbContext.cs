using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.Configuration;
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
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new JobPostingConfiguration());
            builder.ApplyConfiguration(new JobApplicationConfiguration());
            builder.ApplyConfiguration(new CandidateSkillConfiguration());
            builder.ApplyConfiguration(new JobOfferConfiguration());
            builder.ApplyConfiguration(new OffersConfiguration());
            builder.ApplyConfiguration(new JobStepsConfiguration());
            builder.ApplyConfiguration(new CityDataSeedConfiguration());
            builder.ApplyConfiguration(new PersonalizedJobPostingsConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
