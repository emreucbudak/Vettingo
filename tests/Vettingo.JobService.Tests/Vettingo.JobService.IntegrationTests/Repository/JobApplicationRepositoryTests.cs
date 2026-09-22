using FluentAssertions;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Domain.Enums;
using Vettingo.JobService.IntegrationTests;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Repository
{
    public class JobApplicationRepositoryTests : IClassFixture<PostgreSqlContainerFixture>
    {
        private readonly PostgreSqlContainerFixture _fixture;

        public JobApplicationRepositoryTests(PostgreSqlContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_ShouldFilterByCandidateAndOrderByAppliedAt()
        {
            await using JobDbContext context = _fixture.CreateDbContext();
            var repository = new JobApplicationRepository(context);
            var candidateId = Guid.NewGuid();
            var older = CreateApplication(candidateId, DateTime.UtcNow.AddDays(-2));
            var newer = CreateApplication(candidateId, DateTime.UtcNow.AddDays(-1));
            var other = CreateApplication(Guid.NewGuid(), DateTime.UtcNow);
            foreach (var application in new[] { older, newer, other })
            {
                var posting = new JobPosting();
                posting.CreateJobPosting(Guid.NewGuid(), Guid.NewGuid().ToString(), "Description", "Requirements", "Responsibilities", "Remote",
                    EmploymentType.FullTime, WorkingModel.Remote, ExperienceLevel.Mid, null, null, null, JobPostingStatus.Active);
                application.CreateApplication(application.CandidateId, posting.Id, application.AppliedAt, application.Status);
                context.JobPostings.Add(posting);
            }
            await repository.AddAsync(older);
            await repository.AddAsync(newer);
            await repository.AddAsync(other);
            await repository.SaveChangesAsync();

            var result = (await repository.GetAllAsync(candidateId)).ToList();

            result.Should().HaveCount(2);
            result.Select(application => application.Id).Should().ContainInOrder(newer.Id, older.Id);
        }

        [Fact]
        public async Task Statistics_ShouldJoinCompanyPostingsInTheSameDatabase()
        {
            await using var context = _fixture.CreateDbContext();
            var companyId = Guid.NewGuid();
            var ownPosting = NewPosting(companyId);
            var otherPosting = NewPosting(Guid.NewGuid());
            context.JobPostings.AddRange(ownPosting, otherPosting);
            var active = new JobApplication();
            active.CreateApplication(Guid.NewGuid(), ownPosting.Id, DateTime.UtcNow, ApplicationStatus.Submitted);
            var rejected = new JobApplication();
            rejected.CreateApplication(Guid.NewGuid(), ownPosting.Id, DateTime.UtcNow, ApplicationStatus.Rejected);
            var other = new JobApplication();
            other.CreateApplication(Guid.NewGuid(), otherPosting.Id, DateTime.UtcNow, ApplicationStatus.Submitted);
            context.JobApplications.AddRange(active, rejected, other);
            await context.SaveChangesAsync();

            var result = await new JobApplicationRepository(context).GetStatisticsAsync(companyId);
            result.TotalApplications.Should().Be(2);
            result.ActiveApplications.Should().Be(1);
        }

        private static JobPosting NewPosting(Guid companyId)
        {
            var posting = new JobPosting();
            posting.CreateJobPosting(companyId, Guid.NewGuid().ToString(), "Description", "Requirements", "Responsibilities", "Remote",
                EmploymentType.FullTime, WorkingModel.Remote, ExperienceLevel.Mid, null, null, null, JobPostingStatus.Active);
            return posting;
        }

        private static JobApplication CreateApplication(Guid candidateId, DateTime appliedAt)
        {
            JobApplication application = new();
            application.CreateApplication(candidateId, Guid.NewGuid(), appliedAt, ApplicationStatus.Submitted);
            return application;
        }
    }
}
