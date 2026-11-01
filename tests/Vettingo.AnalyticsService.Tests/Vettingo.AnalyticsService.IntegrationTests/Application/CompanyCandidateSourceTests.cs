using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyCandidateSource;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;
using Vettingo.AnalyticsService.Persistence.Repository;

namespace Vettingo.AnalyticsService.IntegrationTests.Application;

public sealed class CompanyCandidateSourceTests(PostgreSqlContainerFixture fixture)
    : IClassFixture<PostgreSqlContainerFixture>
{
    [Fact]
    public async Task Query_Loads_Only_Requested_Company_Source_And_CandidateCount()
    {
        await using var context = fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var companyId = Guid.NewGuid();
        AddSource(context, companyId, 30, 70, 428);
        AddSource(context, Guid.NewGuid(), 90, 10, 999);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var response = await Handler(context).Handle(new(companyId), CancellationToken.None);

        response.Scout.Should().Be(30);
        response.Application.Should().Be(70);
        response.CandidateCount.Should().Be(428);
        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    private static GetCompanyCandidateSourceQueryHandler Handler(AnalyticsDbContext context) => new(
        new AnalyticsRepository(context), NullLogger<GetCompanyCandidateSourceQueryHandler>.Instance);

    private static void AddSource(AnalyticsDbContext context, Guid companyId, int scout, int application, int candidateCount)
    {
        var source = new CandidateSource();
        source.SetId();
        context.Entry(source).Property(x => x.Scout).CurrentValue = scout;
        context.Entry(source).Property(x => x.Application).CurrentValue = application;
        context.Entry(source).Property(x => x.CandidateCount).CurrentValue = candidateCount;
        var statistic = new CompanyStatistic();
        statistic.CreateCompanyStatistic(companyId);
        context.Entry(statistic).Property(x => x.CandidateSourceId).CurrentValue = source.Id;
        context.AddRange(source, statistic);
    }
}
