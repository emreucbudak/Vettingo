using System.Security.Claims;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.JobService.API.Controllers;

namespace Vettingo.JobService.UnitTests.Application;

public sealed class PersonalizedJobPostingsControllerTests
{
    [Theory]
    [InlineData(ClaimTypes.NameIdentifier)]
    [InlineData("sub")]
    public void User_Endpoint_Should_Return_Only_Ok_For_A_Valid_Token_User(string claimType)
    {
        var mediator = Substitute.For<IMediator>();

        var result = Controller(mediator, Guid.NewGuid().ToString(), claimType).GetByUser();

        result.Should().BeOfType<OkResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void User_Endpoint_Should_Reject_Missing_Or_Invalid_Token_UserId(string? subject)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, subject);

        controller.GetByUser().Should().BeOfType<UnauthorizedResult>();
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
