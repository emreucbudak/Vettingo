using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using FlashMediator;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Vettingo.AuthService.API.Controllers;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;

namespace Vettingo.AuthService.UnitTests;

public sealed class TwoFactorLoginControllerTests
{
    [Fact]
    public void Endpoint_Should_Require_Only_TempLogin_Role_And_Default_Scheme()
    {
        var attribute = typeof(AuthController).GetMethod(nameof(AuthController.TwoFactorLogin))!
            .GetCustomAttribute<AuthorizeAttribute>()!;

        attribute.AuthenticationSchemes.Should().BeNull();
        attribute.Roles.Should().Be("TempLogin");
    }

    [Theory]
    [InlineData(ClaimTypes.Email)]
    [InlineData("email")]
    public async Task Endpoint_Should_Use_Token_Email_Instead_Of_Request_Email(string claimType)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, new Claim(claimType, "actual@example.com"));

        await controller.TwoFactorLogin(new() { Code = 123456, Email = "other@example.com" }, CancellationToken.None);

        var command = (TwoFactorLoginCommandRequest)mediator.ReceivedCalls().Single().GetArguments()[0]!;
        command.Code.Should().Be(123456);
        command.Email.Should().Be("actual@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Endpoint_Should_Reject_Missing_Token_Email(string? email)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, email is null ? null : new Claim("email", email));

        var result = await controller.TwoFactorLogin(new() { Code = 123456, Email = "other@example.com" }, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Request_Should_Ignore_Email_In_Json()
    {
        var request = JsonSerializer.Deserialize<TwoFactorLoginCommandRequest>(
            """{"code":123456,"email":"other@example.com"}""", new JsonSerializerOptions(JsonSerializerDefaults.Web));

        request!.Code.Should().Be(123456);
        request.Email.Should().BeEmpty();
    }

    private static AuthController Controller(IMediator mediator, Claim? claim) => new(mediator)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claim is null ? [] : [claim], "test"))
            }
        }
    };
}
