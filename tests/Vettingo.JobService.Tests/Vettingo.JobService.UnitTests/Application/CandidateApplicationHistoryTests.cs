using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Query.GetAll;

namespace Vettingo.JobService.UnitTests.Application;

public class CandidateApplicationHistoryTests
{
    [Theory]
    [InlineData(ClaimTypes.NameIdentifier)]
    [InlineData("sub")]
    public async Task GetAll_ShouldUseTokenCandidateInsteadOfQueryCandidate(string claimType)
    {
        var mediator = Substitute.For<IMediator>();
        var candidateId = Guid.NewGuid();
        var controller = CreateController(mediator, new Claim(claimType, candidateId.ToString()));

        await controller.GetAll(new GetJobApplicationsQueryRequest { CandidateId = Guid.NewGuid() });

        var request = mediator.ReceivedCalls().Single().GetArguments()[0];
        request.Should().BeOfType<GetJobApplicationsQueryRequest>()
            .Which.CandidateId.Should().Be(candidateId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task GetAll_ShouldRejectMissingOrInvalidTokenId(string? subject)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = CreateController(mediator,
            subject is null ? null : new Claim("sub", subject));

        var result = await controller.GetAll(new GetJobApplicationsQueryRequest());

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    private static JobApplicationsController CreateController(IMediator mediator, Claim? subject)
    {
        var claims = new List<Claim> { new("Role", "Candidate") };
        if (subject is not null) claims.Add(subject);
        return new JobApplicationsController(mediator)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test", "sub", "Role"))
                }
            }
        };
    }
}
