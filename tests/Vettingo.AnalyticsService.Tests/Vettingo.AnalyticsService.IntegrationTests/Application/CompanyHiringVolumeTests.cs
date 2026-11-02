using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyHiringVolume;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;
using Vettingo.AnalyticsService.Persistence.Repository;

namespace Vettingo.AnalyticsService.IntegrationTests.Application;

public sealed class CompanyHiringVolumeTests(PostgreSqlContainerFixture fixture)
    : IClassFixture<PostgreSqlContainerFixture>
{
    [Fact]
    public async Task Query_Returns_Last_Six_Company_Volumes_In_CreatedAt_Order()
    {
        await using var context = fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var companyId = Guid.NewGuid();
        foreach (var (year, month, count, createdDay) in new[]
        {
            (2026, 3, 130, 7), (2025, 10, 10, 1), (2026, 1, 0, 3), (2025, 12, 30, 8),
            (2026, 5, 50, 2), (2025, 11, 20, 6), (2026, 4, 40, 5), (2026, 2, 25, 4)
        })
            AddStatistic(context, companyId, year, month, count, new DateTime(2026, 10, createdDay, 0, 0, 0, DateTimeKind.Utc));
        AddStatistic(context, Guid.NewGuid(), 2027, 1, 999);
        await context.SaveChangesAsync();
        context.CompanyStatistics.Local.Single(x => x.CompanyId == companyId).HiringVolumes.Should().HaveCount(8);
        context.ChangeTracker.Clear();

        var statistic = await new AnalyticsRepository(context).GetCompanyStatisticByCompanyIdAsync(companyId);
        statistic!.HiringVolumes.Should().HaveCount(8);

        var handler = new GetCompanyHiringVolumeQueryHandler(
            new AnalyticsRepository(context), NullLogger<GetCompanyHiringVolumeQueryHandler>.Instance);
        var response = (await handler.Handle(new(companyId), CancellationToken.None)).ToList();

        response.Select(x => (x.Year, x.Month, x.HireCount)).Should().Equal(
            (2026, 1, 0), (2026, 2, 25), (2026, 4, 40),
            (2025, 11, 20), (2026, 3, 130), (2025, 12, 30));
        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task Query_Returns_Empty_For_Company_Without_Statistics()
    {
        await using var context = fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        AddStatistic(context, Guid.NewGuid(), 2026, 1, 10);
        await context.SaveChangesAsync();

        var repository = new AnalyticsRepository(context);
        var companyId = Guid.NewGuid();
        (await repository.GetCompanyStatisticByCompanyIdAsync(companyId)).Should().BeNull();
        var handler = new GetCompanyHiringVolumeQueryHandler(repository, NullLogger<GetCompanyHiringVolumeQueryHandler>.Instance);
        (await handler.Handle(new(companyId), CancellationToken.None)).Should().BeEmpty();
    }

    [Fact]
    public async Task Repository_Returns_All_When_Fewer_Than_Six_Records_Exist()
    {
        await using var context = fixture.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        var companyId = Guid.NewGuid();
        AddStatistic(context, companyId, 2026, 2, 30);
        AddStatistic(context, companyId, 2026, 1, 0);
        await context.SaveChangesAsync();

        var statistic = await new AnalyticsRepository(context).GetCompanyStatisticByCompanyIdAsync(companyId);
        statistic.Should().NotBeNull();
        statistic!.GetLatestHiringVolumes().Select(x => (x.Month, x.HireCount)).Should().Equal((1, 0), (2, 30));
    }

    private static void AddStatistic(AnalyticsDbContext context, Guid companyId, int year, int month, int count, DateTime? createdAt = null)
    {
        var volume = new HiringVolume();
        volume.SetId();
        context.Entry(volume).Property(x => x.Year).CurrentValue = year;
        context.Entry(volume).Property(x => x.Month).CurrentValue = month;
        context.Entry(volume).Property(x => x.HireCount).CurrentValue = count;
        context.Entry(volume).Property(x => x.CreatedAt).CurrentValue = createdAt ?? new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var statistic = context.CompanyStatistics.Local.SingleOrDefault(x => x.CompanyId == companyId);
        if (statistic is null)
        {
            var source = new CandidateSource();
            source.SetId();
            statistic = new CompanyStatistic();
            statistic.CreateCompanyStatistic(companyId);
            context.Entry(statistic).Property(x => x.CandidateSourceId).CurrentValue = source.Id;
            context.AddRange(source, statistic);
        }
        statistic.HiringVolumes.Add(volume);
        context.Add(volume);
    }
}
