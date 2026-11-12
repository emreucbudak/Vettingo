using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class CreateJobPostingControllerTests
{
    [Fact]
    public async Task Create_Should_Use_Token_Company_Id_Instead_Of_Body_Value()
    {
        var mediator = Substitute.For<IMediator>();
        var companyId = Guid.NewGuid();
        var request = new CreateJobPostingCommandRequest
        {
            CompanyId = Guid.NewGuid(), Title = "Developer",
            JobSteps = [new() { Order = 1, StepType = StepType.Exam, StartedDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2026, 1, 2) }]
        };
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString())
            .CreateJobPosting(request, cancellation.Token);

        result.Should().BeOfType<OkResult>();
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        var sent = (CreateJobPostingCommandRequest)arguments[0]!;
        sent.CompanyId.Should().Be(companyId);
        sent.Title.Should().Be(request.Title);
        sent.JobSteps.Should().BeEquivalentTo(request.JobSteps);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Create_Should_Reject_Missing_Or_Invalid_Company_Id_Claim(string? companyId)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, companyId)
            .CreateJobPosting(new(), CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    private static JobPostingController Controller(IMediator mediator, string? companyId)
    {
        var claims = new List<Claim>();
        if (companyId is not null) claims.Add(new Claim("companyId", companyId));
        return new(mediator)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
                }
            }
        };
    }
}
