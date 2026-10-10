using System.Reflection;
using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.InterviewService.API.Controllers;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;

namespace Vettingo.InterviewService.UnitTests.Application;

public sealed class InterviewsByDateControllerTests
{
    [Fact]
    public async Task Should_Forward_Selected_Date_Token_Company_And_Cancellation_To_Query()
    {
        var companyId = Guid.NewGuid();
        var date = new DateOnly(2026, 10, 11);
        IEnumerable<GetInterviewsByDateQueryResponse> response = [new() { Name = "Ayşe" }];
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetInterviewsByDateQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString()).GetCompanyInterviewsByDate(date, cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetInterviewsByDateQueryRequest)arguments[0]!).Should().Be(new GetInterviewsByDateQueryRequest(companyId, date));
        arguments[1].Should().Be(cancellation.Token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Should_Reject_Missing_Or_Invalid_Token_Company(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, companyId).GetCompanyInterviewsByDate(new(2026, 10, 11), CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Reject_Missing_Date()
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, Guid.NewGuid().ToString()).GetCompanyInterviewsByDate(default, CancellationToken.None);

        result.Should().BeOfType<BadRequestObjectResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Should_Allow_Company_And_Hr_At_The_Expected_Route()
    {
        var method = typeof(InterviewController).GetMethod(nameof(InterviewController.GetCompanyInterviewsByDate))!;

        method.GetCustomAttribute<AuthorizeAttribute>()!.Roles!.Split(',')
            .Should().BeEquivalentTo("Company", "Human Resources");
        method.GetCustomAttribute<HttpGetAttribute>()!.Template.Should().Be("company/by-date");
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
