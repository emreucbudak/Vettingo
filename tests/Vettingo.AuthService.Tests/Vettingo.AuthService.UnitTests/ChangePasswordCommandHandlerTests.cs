using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ChangePassword;
using Vettingo.AuthService.Application.Validations;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class ChangePasswordCommandHandlerTests
{
    private const string OldPassword = "OldPass1!";
    private const string NewPassword = "NewPass2!";
    private readonly User user = new() { Id = Guid.NewGuid(), Email = "user@example.com", UserName = "user@example.com" };
    private readonly IUserStore<User> store = Substitute.For<IUserStore<User>, IUserPasswordStore<User>, IUserEmailStore<User>>();
    private readonly UserManager<User> manager;
    private readonly ChangePasswordCommandHandler handler;

    public ChangePasswordCommandHandlerTests()
    {
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, OldPassword);
        var passwordStore = (IUserPasswordStore<User>)store;
        var emailStore = (IUserEmailStore<User>)store;
        emailStore.FindByEmailAsync("USER@EXAMPLE.COM", Arg.Any<CancellationToken>()).Returns(user);
        emailStore.GetEmailAsync(user, Arg.Any<CancellationToken>()).Returns(user.Email);
        store.GetUserNameAsync(user, Arg.Any<CancellationToken>()).Returns(user.UserName);
        store.GetUserIdAsync(user, Arg.Any<CancellationToken>()).Returns(user.Id.ToString());
        passwordStore.GetPasswordHashAsync(user, Arg.Any<CancellationToken>()).Returns(_ => user.PasswordHash);
        passwordStore.SetPasswordHashAsync(user, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call => { user.PasswordHash = call.ArgAt<string>(1); return Task.CompletedTask; });
        store.UpdateAsync(user, Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
        manager = new UserManager<User>(store, Options.Create(new IdentityOptions()), hasher,
            [], [new PasswordValidator<User>()], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
        handler = new ChangePasswordCommandHandler(manager, new ChangePasswordCommandRequestValidator());
    }

    [Fact]
    public async Task Handle_ShouldReplaceHashAndAcceptOnlyNewPassword()
    {
        string? oldHash = user.PasswordHash;
        await handler.Handle(Request(), CancellationToken.None);

        user.PasswordHash.Should().NotBe(oldHash).And.NotBe(NewPassword);
        (await manager.CheckPasswordAsync(user, NewPassword)).Should().BeTrue();
        (await manager.CheckPasswordAsync(user, OldPassword)).Should().BeFalse();
        await store.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldLeavePasswordUnchangedWhenCurrentPasswordIsWrong()
    {
        string? oldHash = user.PasswordHash;
        Func<Task> action = () => handler.Handle(Request() with { CurrentPassword = "Wrong1!" }, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequestException>().WithMessage("Mevcut şifreniz yanlış.");
        user.PasswordHash.Should().Be(oldHash);
        await store.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("weakpassword")]
    [InlineData("NewPassword!")]
    [InlineData("NewPassword2")]
    public async Task Handle_ShouldEnforceIdentityPasswordPolicy(string newPassword)
    {
        string? oldHash = user.PasswordHash;
        Func<Task> action = () => handler.Handle(Request() with { NewPassword = newPassword }, CancellationToken.None);
        await action.Should().ThrowAsync<BadRequestException>();
        user.PasswordHash.Should().Be(oldHash);
        await store.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short")]
    [InlineData(OldPassword)]
    public async Task Handle_ShouldRejectEmptyShortOrUnchangedNewPassword(string newPassword)
    {
        Func<Task> action = () => handler.Handle(Request() with { NewPassword = newPassword }, CancellationToken.None);
        await action.Should().ThrowAsync<ValidationException>();
        await store.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRejectMissingUser()
    {
        Func<Task> action = () => handler.Handle(Request() with { Email = "missing@example.com" }, CancellationToken.None);
        await action.Should().ThrowAsync<UnauthorizedException>();
        await store.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReportPersistenceFailure()
    {
        store.UpdateAsync(user, Arg.Any<CancellationToken>()).Returns(
            IdentityResult.Failed(new IdentityError { Code = "ConcurrencyFailure" }));
        Func<Task> action = () => handler.Handle(Request(), CancellationToken.None);
        await action.Should().ThrowAsync<BadRequestException>();
    }

    private ChangePasswordCommandRequest Request() => new()
    {
        Email = user.Email!, CurrentPassword = OldPassword, NewPassword = NewPassword
    };
}
