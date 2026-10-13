using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.AnalyticsService.API.Controllers;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Query.GetCompanyCandidateSource;
using Vettingo.AnalyticsService.Domain.Entities;
using Vettingo.AnalyticsService.Persistence.DbContext;
using Vettingo.AnalyticsService.Persistence.Repository;

namespace Vettingo.AnalyticsService.UnitTests.Application;

public sealed class CompanyCandidateSourceTests
{
    [Fact]
    public async Task Query_Loads_Only_Requested_Company_Source_And_CandidateCount()
    {
        await using var context = CreateContext();
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

    [Fact]
    public async Task Query_Returns_Zero_When_Company_Has_No_Statistics()
    {
        await using var context = CreateContext();
        AddSource(context, Guid.NewGuid(), 30, 70, 428);
        await context.SaveChangesAsync();

        var response = await Handler(context).Handle(new(Guid.NewGuid()), CancellationToken.None);

        response.Scout.Should().Be(0);
        response.Application.Should().Be(0);
        response.CandidateCount.Should().Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Endpoint_Uses_Claim_Company_And_Forwards_Cancellation(bool supplyId)
    {
        var companyId = Guid.NewGuid();
        var response = new GetCompanyCandidateSourceQueryResponse { Scout = 30, Application = 70, CandidateCount = 428 };
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCompanyCandidateSourceQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString())
            .GetCompanyCandidateSource(supplyId ? companyId : Guid.Empty, cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetCompanyCandidateSourceQueryRequest)arguments[0]!).CompanyId.Should().Be(companyId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Fact]
    public async Task Endpoint_Rejects_Another_Company()
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, Guid.NewGuid().ToString())
            .GetCompanyCandidateSource(Guid.NewGuid(), CancellationToken.None);
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
            .GetCompanyCandidateSource(Guid.Empty, CancellationToken.None);
        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    private static AnalyticsDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AnalyticsDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

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
