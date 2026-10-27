using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;
using Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class PersonalizedJobPostingsControllerTests
{
    [Theory]
    [InlineData(ClaimTypes.NameIdentifier)]
    [InlineData("sub")]
    public async Task User_Endpoint_Should_Return_Postings_For_Token_User(string claimType)
    {
        var userId = Guid.NewGuid();
        IEnumerable<GetAllPersonalizedJobPostingsQueryResponse> response =
            [new() { JobPostingId = Guid.NewGuid(), Title = "Backend Developer" }];
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<GetAllPersonalizedJobPostingsQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        using var cancellation = new CancellationTokenSource();

        var result = await Controller(mediator, userId.ToString(), claimType).GetByUser(cancellation.Token);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeSameAs(response);
        var arguments = mediator.ReceivedCalls().Single().GetArguments();
        ((GetAllPersonalizedJobPostingsQueryRequest)arguments[0]!).UserId.Should().Be(userId);
        arguments[1].Should().Be(cancellation.Token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task User_Endpoint_Should_Reject_Missing_Or_Invalid_Token_UserId(string? subject)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, subject);

        (await controller.GetByUser(CancellationToken.None)).Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    private static PersonalizedJobPostingsController Controller(IMediator mediator, string? subject,
        string claimType = ClaimTypes.NameIdentifier) => new(mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    subject is null ? [] : [new Claim(claimType, subject)], "test"))
            }
        }
    };
}
