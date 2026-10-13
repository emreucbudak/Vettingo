using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.AnalyticsService.API.Controllers;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyHiringVolume;
using Vettingo.AnalyticsService.Application.Validations;
using Vettingo.AnalyticsService.Application.Repository;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;
using Vettingo.AnalyticsService.Persistence.Repository;

namespace Vettingo.AnalyticsService.UnitTests.Application;

public sealed class CompanyHiringVolumeTests
{
    [Fact]
    public async Task Query_Returns_Last_Six_Company_Volumes_In_CreatedAt_Order()
    {
        await using var context = CreateContext();
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
        await using var context = CreateContext();
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
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        AddStatistic(context, companyId, 2026, 2, 30);
        AddStatistic(context, companyId, 2026, 1, 0);
        await context.SaveChangesAsync();

        var statistic = await new AnalyticsRepository(context).GetCompanyStatisticByCompanyIdAsync(companyId);
        statistic.Should().NotBeNull();
        statistic!.GetLatestHiringVolumes().Select(x => (x.Month, x.HireCount)).Should().Equal((1, 0), (2, 30));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Endpoint_Uses_Authenticated_Company_And_Forwards_Cancellation(bool supplyId)
    {
        var companyId = Guid.NewGuid();
        IEnumerable<GetCompanyHiringVolumeQueryResponse> response = [new() { Year = 2026, Month = 1, HireCount = 30 }];
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCompanyHiringVolumeQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString())
            .GetCompanyHiringVolume(supplyId ? companyId : Guid.Empty, cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetCompanyHiringVolumeQueryRequest)arguments[0]!).CompanyId.Should().Be(companyId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Fact]
    public async Task Endpoint_Rejects_Another_Company()
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, Guid.NewGuid().ToString())
            .GetCompanyHiringVolume(Guid.NewGuid(), CancellationToken.None);
        result.Should().BeOfType<ForbidResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Endpoint_Rejects_Missing_Or_Invalid_Company_Claim(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, companyId)
            .GetCompanyHiringVolume(Guid.Empty, CancellationToken.None);
        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task HybridCache_Reuses_Result_Across_Requests_And_Isolates_Companies()
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        AddStatistic(context, companyId, 2026, 1, 30);
        await context.SaveChangesAsync();
        var statistic = await new AnalyticsRepository(context).GetCompanyStatisticByCompanyIdAsync(companyId);
        var repository = Substitute.For<IAnalyticsRepository>();
        repository.GetCompanyStatisticByCompanyIdAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(statistic));
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(repository);
        services.AddFlashMediator(typeof(GetCompanyHiringVolumeQueryHandler).Assembly);
        services.AddFlashMediatorHybridCache();
        await using var provider = services.BuildServiceProvider();

        await using (var scope = provider.CreateAsyncScope())
        {
            var result = await scope.ServiceProvider.GetRequiredService<IMediator>()
                .Send(new GetCompanyHiringVolumeQueryRequest(companyId));
            result.Single().HireCount.Should().Be(30);
        }
        await using (var scope = provider.CreateAsyncScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var cached = await mediator.Send(new GetCompanyHiringVolumeQueryRequest(companyId));
            cached.Single().HireCount.Should().Be(30);
            var otherCompanyId = Guid.NewGuid();
            var otherCompany = await mediator.Send(new GetCompanyHiringVolumeQueryRequest(otherCompanyId));
            otherCompany.Should().BeEmpty();
            await repository.Received(1).GetCompanyStatisticByCompanyIdAsync(otherCompanyId, Arg.Any<CancellationToken>());
        }
        await repository.Received(1).GetCompanyStatisticByCompanyIdAsync(companyId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Validator_Rejects_Empty_CompanyId()
    {
        var validator = new GetCompanyHiringVolumeQueryRequestValidator();
        validator.Validate(new GetCompanyHiringVolumeQueryRequest(Guid.Empty)).IsValid.Should().BeFalse();
        validator.Validate(new GetCompanyHiringVolumeQueryRequest(Guid.NewGuid())).IsValid.Should().BeTrue();
    }

    private static AnalyticsDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AnalyticsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

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

    private static AnalyticsController Controller(IMediator mediator, string? companyId) => new(mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    companyId is null ? [] : [new Claim("companyId", companyId)], "test"))
            }
        }
    };
}
