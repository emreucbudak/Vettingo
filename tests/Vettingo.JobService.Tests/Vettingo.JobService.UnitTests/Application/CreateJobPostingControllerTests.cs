using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class CreateJobPostingControllerTests
{
    [Fact]
    public async Task Create_Should_Use_Token_Company_Instead_Of_Body_Values()
    {
        var mediator = Substitute.For<IMediator>();
        var companyId = Guid.NewGuid();
        var request = new CreateJobPostingCommandRequest
        {
            CompanyId = Guid.NewGuid(), CompanyName = "Body company", Title = "Developer"
        };
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, companyId.ToString(), "Vettingo Teknoloji")
            .CreateJobPosting(request, cancellation.Token);

        result.Should().BeOfType<OkResult>();
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        var sent = (CreateJobPostingCommandRequest)arguments[0]!;
        sent.CompanyId.Should().Be(companyId);
        sent.CompanyName.Should().Be("Vettingo Teknoloji");
        sent.Title.Should().Be(request.Title);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Theory]
    [InlineData(null, "Vettingo")]
    [InlineData("invalid", "Vettingo")]
    [InlineData("00000000-0000-0000-0000-000000000000", "Vettingo")]
    [InlineData("019a16b1-1234-7000-8000-000000000001", null)]
    [InlineData("019a16b1-1234-7000-8000-000000000001", "")]
    [InlineData("019a16b1-1234-7000-8000-000000000001", " ")]
    public async Task Create_Should_Reject_Missing_Or_Invalid_Company_Claims(string? companyId, string? companyName)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, companyId, companyName)
            .CreateJobPosting(new(), CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    private static JobPostingController Controller(IMediator mediator, string? companyId, string? companyName)
    {
        var claims = new List<Claim>();
        if (companyId is not null) claims.Add(new Claim("companyId", companyId));
        if (companyName is not null) claims.Add(new Claim("companyName", companyName));
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
