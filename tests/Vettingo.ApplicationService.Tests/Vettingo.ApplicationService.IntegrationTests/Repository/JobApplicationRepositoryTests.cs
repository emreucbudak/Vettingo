using FluentAssertions;
using Vettingo.ApplicationService.Domain.Entities;
using Vettingo.ApplicationService.Domain.Enums;
using Vettingo.ApplicationService.IntegrationTests;
using Vettingo.ApplicationService.Persistence.DbContext;
using Vettingo.ApplicationService.Persistence.Repository;

namespace Vettingo.ApplicationService.UnitTests.Repository
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
            await using ApplicationDbContext context = _fixture.CreateDbContext();
            var repository = new JobApplicationRepository(context);
            var candidateId = Guid.NewGuid();
            var older = CreateApplication(candidateId, DateTime.UtcNow.AddDays(-2));
            var newer = CreateApplication(candidateId, DateTime.UtcNow.AddDays(-1));
            var other = CreateApplication(Guid.NewGuid(), DateTime.UtcNow);
            await repository.AddAsync(older);
            await repository.AddAsync(newer);
            await repository.AddAsync(other);
            await repository.SaveChangesAsync();

            var result = (await repository.GetAllAsync(candidateId)).ToList();

            result.Should().HaveCount(2);
            result.Select(application => application.Id).Should().ContainInOrder(newer.Id, older.Id);
        }

        private static JobApplication CreateApplication(Guid candidateId, DateTime appliedAt)
        {
            JobApplication application = new();
            application.CreateApplication(candidateId, Guid.NewGuid(), appliedAt, ApplicationStatus.Submitted);
            return application;
        }
    }
}
