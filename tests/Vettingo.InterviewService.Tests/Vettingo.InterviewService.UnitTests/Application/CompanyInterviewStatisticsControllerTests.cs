using System.Reflection;
using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.InterviewService.API.Controllers;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;

namespace Vettingo.InterviewService.UnitTests.Application;

public sealed class CompanyInterviewStatisticsControllerTests
{
    [Fact]
    public async Task Statistics_Should_Use_Token_Company_And_Forward_Cancellation()
    {
        var companyId = Guid.NewGuid();
        var response = new GetCompanyInterviewStatisticsQueryResponse(8, 4, 2, 1);
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCompanyInterviewStatisticsQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString()).GetCompanyStatistics(cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetCompanyInterviewStatisticsQueryRequest)arguments[0]!).CompanyId.Should().Be(companyId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Statistics_Should_Reject_Missing_Or_Invalid_Token_Company(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, companyId).GetCompanyStatistics(CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Statistics_Should_Allow_Company_And_Hr_Roles_At_The_Expected_Route()
    {
        var method = typeof(InterviewController).GetMethod(nameof(InterviewController.GetCompanyStatistics))!;

        method.GetCustomAttribute<AuthorizeAttribute>()!.Roles!.Split(',')
            .Should().BeEquivalentTo("Company", "Human Resources");
        method.GetCustomAttribute<HttpGetAttribute>()!.Template.Should().Be("company/statistics");
    }

    private static InterviewController Controller(IMediator mediator, string? companyId) => new(mediator)
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
