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
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPassword;

namespace Vettingo.AuthService.UnitTests;

public sealed class ResetPasswordControllerTests
{
    [Fact]
    public void Endpoint_Should_Require_ResetPassword_Role()
    {
        var endpoint = typeof(AuthController).GetMethod(nameof(AuthController.ResetPassword))!;
        endpoint.GetCustomAttribute<AuthorizeAttribute>()!.Roles.Should().Be("ResetPassword");
        endpoint.GetCustomAttribute<HttpPostAttribute>()!.Template.Should().Be("reset-password");
    }

    [Theory]
    [InlineData(ClaimTypes.Email)]
    [InlineData("email")]
    public async Task Endpoint_Should_Copy_Request_With_Token_Email(string claimType)
    {
        var mediator = Substitute.For<IMediator>();
        var request = new ResetPasswordCommandRequest { NewPassword = "NewPass2!", Email = "other@example.com" };

        var result = await Controller(mediator, new Claim(claimType, "actual@example.com"))
            .ResetPassword(request, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        var command = (ResetPasswordCommandRequest)mediator.ReceivedCalls().Single().GetArguments()[0]!;
        command.Email.Should().Be("actual@example.com");
        command.NewPassword.Should().Be(request.NewPassword);
        request.Email.Should().Be("other@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Endpoint_Should_Reject_Missing_Token_Email(string? email)
    {
        var mediator = Substitute.For<IMediator>();

        var result = await Controller(mediator, email is null ? null : new Claim("email", email))
            .ResetPassword(new() { NewPassword = "NewPass2!" }, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Request_Should_Ignore_Client_Email()
    {
        var request = JsonSerializer.Deserialize<ResetPasswordCommandRequest>(
            """{"newPassword":"NewPass2!","email":"other@example.com"}""", new JsonSerializerOptions(JsonSerializerDefaults.Web));

        request!.Email.Should().BeEmpty();
        request.NewPassword.Should().Be("NewPass2!");
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
