using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.AnalyticsService.API.Controllers;
using Vettingo.AnalyticsService.Application.Features.CQRS.CompanyStatics.Query.GetCompanyCandidateSource;

namespace Vettingo.AnalyticsService.UnitTests.Controllers;

public sealed class CompanyCandidateSourceControllerTests
{
    [Fact]
    public async Task Endpoint_Uses_Claim_Company_And_Forwards_Cancellation()
    {
        var companyId = Guid.NewGuid();
        var response = new GetCompanyCandidateSourceQueryResponse { Scout = 30, Application = 70, CandidateCount = 428 };
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCompanyCandidateSourceQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString())
            .GetCompanyCandidateSource(companyId, cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetCompanyCandidateSourceQueryRequest)arguments[0]!).CompanyId.Should().Be(companyId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Fact]
    public async Task Endpoint_Rejects_Empty_CompanyId()
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, Guid.NewGuid().ToString())
            .GetCompanyCandidateSource(Guid.Empty, CancellationToken.None);

        result.Should().BeOfType<ForbidResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
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
