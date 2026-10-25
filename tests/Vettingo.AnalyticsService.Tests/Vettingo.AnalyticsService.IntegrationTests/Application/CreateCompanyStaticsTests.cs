using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Command.CreateCompanyStatics;
using Vettingo.AnalyticsService.Persistence.Repository;

namespace Vettingo.AnalyticsService.IntegrationTests.Application;

public sealed class CreateCompanyStaticsTests(PostgreSqlContainerFixture fixture)
    : IClassFixture<PostgreSqlContainerFixture>
{
    [Fact]
    public async Task Command_Persists_Company_Statistic_With_Required_CandidateSource()
    {
        await using var context = fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var companyId = Guid.NewGuid();
        var handler = new CreateCompanyStaticsCommandHandler(new AnalyticsRepository(context));

        await handler.Handle(new CreateCompanyStaticsCommandRequest { CompanyId = companyId }, CancellationToken.None);
        context.ChangeTracker.Clear();

        var statistic = await context.CompanyStatistics.Include(x => x.CandidateSource).SingleAsync();
        statistic.CompanyId.Should().Be(companyId);
        statistic.Id.Should().NotBeEmpty();
        statistic.CandidateSourceId.Should().NotBeEmpty();
        statistic.CandidateSource.Id.Should().Be(statistic.CandidateSourceId);
        statistic.CandidateSource.CandidateCount.Should().Be(0);
        statistic.TotalJobPostingCount.Should().Be(0);
        (await context.CandidateSources.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Command_Rejects_Empty_CompanyId_Without_Saving()
    {
        await using var context = fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var handler = new CreateCompanyStaticsCommandHandler(new AnalyticsRepository(context));

        var action = () => handler.Handle(new CreateCompanyStaticsCommandRequest(), CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>();
        (await context.CompanyStatistics.CountAsync()).Should().Be(0);
        (await context.CandidateSources.CountAsync()).Should().Be(0);
    }
}
