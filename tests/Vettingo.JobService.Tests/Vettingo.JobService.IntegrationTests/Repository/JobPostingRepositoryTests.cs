using FluentAssertions;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.IntegrationTests;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Repository
{
    public class JobPostingRepositoryTests : IClassFixture<PostgreSqlContainerFixture>
    {
        private readonly PostgreSqlContainerFixture _fixture;

        public JobPostingRepositoryTests(PostgreSqlContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task AddJobPostingAsync_Then_GetJobPostingByIdAsync_Should_Return_JobPosting()
        {
            await using JobDbContext context = _fixture.CreateDbContext();
            var repository = new JobPostingRepository(context);
            var jobPosting = CreateJobPosting(Guid.NewGuid(), "Backend Developer");

            await repository.AddJobPostingAsync(jobPosting);
            await repository.SaveChangesAsync();

            var result = await repository.GetJobPostingByIdAsync(jobPosting.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(jobPosting.Id);
            result.Title.Should().Be("Backend Developer");
        }

        [Fact]
        public async Task GetJobPostingsByCompanyIdAsync_Should_Return_Only_Company_JobPostings()
        {
            await using JobDbContext context = _fixture.CreateDbContext();
            var repository = new JobPostingRepository(context);
            var companyId = Guid.NewGuid();
            var otherCompanyId = Guid.NewGuid();

            await repository.AddJobPostingAsync(CreateJobPosting(companyId, "Backend Developer"));
            await repository.AddJobPostingAsync(CreateJobPosting(companyId, "Frontend Developer"));
            await repository.AddJobPostingAsync(CreateJobPosting(otherCompanyId, "Mobile Developer"));
            await repository.SaveChangesAsync();

            var result = (await repository.GetJobPostingsByCompanyIdAsync(companyId)).ToList();

            result.Should().HaveCount(2);
            result.Should().OnlyContain(jobPosting => jobPosting.CompanyId == companyId);
        }

        private static JobPosting CreateJobPosting(Guid companyId, string title)
        {
            JobPosting jobPosting = new();
            jobPosting.CreateJobPosting(
                companyId,
                title,
                "Build and maintain backend services.",
                "C# and PostgreSQL experience.",
                "Deliver reliable service features.",
                "Remote",
                EmploymentType.FullTime,
                WorkingModel.Remote,
                ExperienceLevel.Mid,
                50000m,
                70000m,
                DateTime.UtcNow.AddDays(30),
                JobPostingStatus.Published);

            return jobPosting;
        }
    }
}
