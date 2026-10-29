using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPassword;
using Vettingo.AuthService.Application.Validations;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Persistence.Registration;

namespace Vettingo.AuthService.UnitTests;

public sealed class ResetPasswordCommandHandlerTests : IDisposable
{
    private const string OldPassword = "OldPass1!";
    private const string NewPassword = "NewPass2!";
    private readonly User user = new()
    {
        Id = Guid.NewGuid(), Email = "user@example.com", UserName = "user@example.com", SecurityStamp = "old-stamp"
    };
    private readonly IUserStore<User> store = (IUserStore<User>)Substitute.For(
        [typeof(IUserStore<User>), typeof(IUserPasswordStore<User>), typeof(IUserEmailStore<User>), typeof(IUserSecurityStampStore<User>)], []);
    private readonly UserManager<User> manager;
    private readonly ResetPasswordCommandHandler handler;

    public ResetPasswordCommandHandlerTests()
    {
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, OldPassword);
        ((IUserEmailStore<User>)store).FindByEmailAsync("USER@EXAMPLE.COM", Arg.Any<CancellationToken>()).Returns(user);
        store.GetUserIdAsync(user, Arg.Any<CancellationToken>()).Returns(user.Id.ToString());
        store.GetUserNameAsync(user, Arg.Any<CancellationToken>()).Returns(user.UserName);
        var passwordStore = (IUserPasswordStore<User>)store;
        passwordStore.GetPasswordHashAsync(user, Arg.Any<CancellationToken>()).Returns(_ => user.PasswordHash);
        passwordStore.SetPasswordHashAsync(user, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call => { user.PasswordHash = call.ArgAt<string>(1); return Task.CompletedTask; });
        var stampStore = (IUserSecurityStampStore<User>)store;
        stampStore.GetSecurityStampAsync(user, Arg.Any<CancellationToken>()).Returns(_ => user.SecurityStamp);
        stampStore.SetSecurityStampAsync(user, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(call => { user.SecurityStamp = call.ArgAt<string>(1); return Task.CompletedTask; });
        store.UpdateAsync(user, Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
        manager = new UserManager<User>(store, Options.Create(new IdentityOptions()), hasher,
            [], [new PasswordValidator<User>()], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
        manager.RegisterTokenProvider(TokenOptions.DefaultProvider, new DataProtectorTokenProvider<User>(
            new EphemeralDataProtectionProvider(), Options.Create(new DataProtectionTokenProviderOptions()),
            NullLogger<DataProtectorTokenProvider<User>>.Instance));
        handler = new ResetPasswordCommandHandler(manager, new ResetPasswordCommandRequestValidator());
    }

    [Fact]
    public async Task Reset_Should_Replace_Hash_And_Security_Stamp_Without_Current_Password()
    {
        string? oldHash = user.PasswordHash;
        string? oldStamp = user.SecurityStamp;

        await handler.Handle(Request(), CancellationToken.None);

        user.PasswordHash.Should().NotBe(oldHash).And.NotBe(NewPassword);
        user.SecurityStamp.Should().NotBe(oldStamp);
        (await manager.CheckPasswordAsync(user, NewPassword)).Should().BeTrue();
        (await manager.CheckPasswordAsync(user, OldPassword)).Should().BeFalse();
        await store.Received(1).UpdateAsync(user, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("weakpassword")]
    [InlineData("NewPassword!")]
    [InlineData("NewPassword2")]
    public async Task Reset_Should_Enforce_Identity_Password_Policy(string password)
    {
        string? oldHash = user.PasswordHash;
        Func<Task> action = () => handler.Handle(Request() with { NewPassword = password }, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequestException>();
        user.PasswordHash.Should().Be(oldHash);
        await store.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData("Short")]
    [InlineData(null)]
    public async Task Reset_Should_Reject_Empty_Or_Short_Password_Before_Finding_User(string? password)
    {
        Func<Task> action = () => handler.Handle(Request() with { NewPassword = password! }, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
        store.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    [InlineData(null)]
    public async Task Reset_Should_Reject_Invalid_Email(string? email)
    {
        Func<Task> action = () => handler.Handle(Request() with { Email = email! }, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
        store.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Reset_Should_Reject_Missing_User()
    {
        Func<Task> action = () => handler.Handle(Request() with { Email = "missing@example.com" }, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
        await store.DidNotReceive().UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Reset_Should_Report_Persistence_Failure()
    {
        store.UpdateAsync(user, Arg.Any<CancellationToken>()).Returns(
            IdentityResult.Failed(new IdentityError { Code = "ConcurrencyFailure" }));
        Func<Task> action = () => handler.Handle(Request(), CancellationToken.None);

        await action.Should().ThrowAsync<BadRequestException>();
    }

    [Fact]
    public async Task Identity_Store_And_Token_Provider_Registrations_Should_Support_Password_Reset_Tokens()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection().UseEphemeralDataProtectionProvider();
        services.SaveDb(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unused;Username=unused;Password=unused"
        }).Build());
        services.AddIdentityCore<User>().AddDefaultTokenProviders();
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var registeredManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        string token = await registeredManager.GeneratePasswordResetTokenAsync(user);

        (await registeredManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider,
            UserManager<User>.ResetPasswordTokenPurpose, token)).Should().BeTrue();
    }

    private ResetPasswordCommandRequest Request() => new() { Email = user.Email!, NewPassword = NewPassword };

    public void Dispose() => manager.Dispose();
}
