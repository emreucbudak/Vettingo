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
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ChangePassword;

namespace Vettingo.AuthService.UnitTests;

public sealed class ChangePasswordControllerTests
{
    [Theory]
    [InlineData(ClaimTypes.Email)]
    [InlineData("email")]
    public async Task ChangePassword_ShouldUseTokenEmailInsteadOfRequestEmail(string claimType)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, new Claim(claimType, "actual@example.com"));
        var result = await controller.ChangePassword(new ChangePasswordCommandRequest
        {
            Email = "other@example.com", CurrentPassword = "OldPass1!", NewPassword = "NewPass2!"
        }, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        var command = (ChangePasswordCommandRequest)mediator.ReceivedCalls().Single().GetArguments()[0]!;
        command.Email.Should().Be("actual@example.com");
        command.CurrentPassword.Should().Be("OldPass1!");
        command.NewPassword.Should().Be("NewPass2!");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ChangePassword_ShouldRejectMissingEmail(string? email)
    {
        var mediator = Substitute.For<IMediator>();
        var controller = Controller(mediator, email is null ? null : new Claim("email", email));
        var result = await controller.ChangePassword(new ChangePasswordCommandRequest(), CancellationToken.None);
        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void ChangePassword_ShouldAllowOnlyTheThreeAccountRoles()
    {
        var attribute = typeof(AuthController).GetMethod(nameof(AuthController.ChangePassword))!
            .GetCustomAttribute<AuthorizeAttribute>()!;
        attribute.Roles!.Split(',').Should().BeEquivalentTo("Candidate", "Company", "Human Resources");
    }

    [Fact]
    public void Request_ShouldIgnoreEmailSuppliedInJson()
    {
        var request = JsonSerializer.Deserialize<ChangePasswordCommandRequest>(
            """{"email":"other@example.com","currentPassword":"OldPass1!","newPassword":"NewPass2!"}""",
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        request!.Email.Should().BeEmpty();
        request.CurrentPassword.Should().Be("OldPass1!");
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
