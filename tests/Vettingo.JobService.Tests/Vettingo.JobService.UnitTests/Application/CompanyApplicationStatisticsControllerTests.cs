using System.Reflection;
using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetCompanyStatistics;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class CompanyApplicationStatisticsControllerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Statistics_Should_Return_Response_For_Token_Company(bool supplyCompanyId)
    {
        var companyId = Guid.NewGuid();
        var response = new GetCompanyApplicationStatisticsQueryResponse(8, 2, 2, 1, 1);
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetCompanyApplicationStatisticsQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString()).GetCompanyStatistics(
            new() { CompanyId = supplyCompanyId ? companyId : Guid.Empty }, cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetCompanyApplicationStatisticsQueryRequest)arguments[0]!).CompanyId.Should().Be(companyId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Fact]
    public async Task Statistics_Should_Reject_Another_Company()
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, Guid.NewGuid().ToString()).GetCompanyStatistics(
            new() { CompanyId = Guid.NewGuid() }, CancellationToken.None);

        result.Should().BeOfType<ForbidResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Statistics_Should_Reject_Missing_Or_Invalid_Token_Company(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, companyId).GetCompanyStatistics(
            new() { CompanyId = Guid.NewGuid() }, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Statistics_Should_Allow_Company_And_Hr_Roles()
    {
        var attribute = typeof(JobApplicationsController).GetMethod(nameof(JobApplicationsController.GetCompanyStatistics))!
            .GetCustomAttribute<AuthorizeAttribute>()!;

        attribute.Roles!.Split(',').Should().BeEquivalentTo("Company", "Human Resources");
    }

    private static JobApplicationsController Controller(IMediator mediator, string? companyId) => new(mediator)
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
