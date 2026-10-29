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
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;

namespace Vettingo.AuthService.UnitTests;

public sealed class RegisterControllerTests
{
    [Fact]
    public void Only_Confirmation_Endpoint_Should_Require_TempRegister_Role()
    {
        typeof(AuthController).GetMethod(nameof(AuthController.TempRegister))!
            .GetCustomAttribute<AuthorizeAttribute>().Should().BeNull();
        typeof(AuthController).GetMethod(nameof(AuthController.Register))!
            .GetCustomAttribute<AuthorizeAttribute>()!.Roles.Should().Be("TempRegister");
    }

    [Fact]
    public async Task TempRegister_Should_Send_Request_To_Mediator_And_Return_Token()
    {
        var mediator = Substitute.For<IMediator>();
        var request = new TempRegisterCommandRequest { Email = "user@example.com", Password = "Strong1!", Role = "Candidate" };
        mediator.Send(request, CancellationToken.None).Returns(new TempRegisterCommandResponse { TempToken = "temp-token" });

        var result = await Controller(mediator, null).TempRegister(request, CancellationToken.None);

        result.Should().BeOfType<OkObjectResult>().Which.Value.Should()
            .BeEquivalentTo(new TempRegisterCommandResponse { TempToken = "temp-token" });
        await mediator.Received(1).Send(request, CancellationToken.None);
    }

    [Theory]
    [InlineData(ClaimTypes.Email)]
    [InlineData("email")]
    public async Task Register_Should_Copy_Request_With_Token_Email(string claimType)
    {
        var mediator = Substitute.For<IMediator>();
        var request = new RegisterCommandRequest { Code = 123456, Email = "other@example.com" };

        var result = await Controller(mediator, new Claim(claimType, "actual@example.com")).Register(request, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
        var command = (RegisterCommandRequest)mediator.ReceivedCalls().Single().GetArguments()[0]!;
        command.Code.Should().Be(request.Code);
        command.Email.Should().Be("actual@example.com");
        request.Email.Should().Be("other@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Register_Should_Reject_Missing_Token_Email(string? email)
    {
        var mediator = Substitute.For<IMediator>();
        var result = await Controller(mediator, email is null ? null : new Claim("email", email))
            .Register(new() { Code = 123456 }, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedResult>();
        mediator.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Register_Request_Should_Ignore_Client_Email()
    {
        var request = JsonSerializer.Deserialize<RegisterCommandRequest>(
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
