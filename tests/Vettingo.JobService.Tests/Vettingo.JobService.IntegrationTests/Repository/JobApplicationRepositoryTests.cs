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
            var olderPosting = NewPosting(Guid.NewGuid());
            var newerPosting = NewPosting(Guid.NewGuid());
            var otherPosting = NewPosting(Guid.NewGuid());
            context.JobPostings.AddRange(olderPosting, newerPosting, otherPosting);
            await context.SaveChangesAsync();

            var older = CreateApplication(candidateId, olderPosting.Id, DateTime.UtcNow.AddDays(-2));
            var newer = CreateApplication(candidateId, newerPosting.Id, DateTime.UtcNow.AddDays(-1));
            var other = CreateApplication(Guid.NewGuid(), otherPosting.Id, DateTime.UtcNow);
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
            await context.SaveChangesAsync();

            var active = new JobApplication();
            active.CreateApplication(Guid.NewGuid(), ownPosting.Id, DateTime.UtcNow, ApplicationStatus.Başvuruldu);
            var rejected = new JobApplication();
            rejected.CreateApplication(Guid.NewGuid(), ownPosting.Id, DateTime.UtcNow, ApplicationStatus.Elendi);
            var other = new JobApplication();
            other.CreateApplication(Guid.NewGuid(), otherPosting.Id, DateTime.UtcNow, ApplicationStatus.Başvuruldu);
            context.JobApplications.AddRange(active, rejected, other);
            await context.SaveChangesAsync();

            var result = await new JobApplicationRepository(context).GetStatisticsAsync(companyId);
            result.TotalApplications.Should().Be(2);
            result.ActiveApplications.Should().Be(1);
        }

        private static JobPosting NewPosting(Guid companyId)
        {
            var posting = new JobPosting();
            posting.CreateJobPosting(companyId, Guid.NewGuid().ToString(), 34,
                EmploymentType.FullTime, WorkingModel.Remote, 2, 5, 0, 0, [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]);
            return posting;
        }

        private static JobApplication CreateApplication(Guid candidateId, Guid jobPostingId, DateTime appliedAt)
        {
            JobApplication application = new();
            application.CreateApplication(candidateId, jobPostingId, appliedAt, ApplicationStatus.Başvuruldu);
            return application;
        }
    }
}
