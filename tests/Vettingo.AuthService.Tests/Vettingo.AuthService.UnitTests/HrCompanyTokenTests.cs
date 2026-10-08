using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.RefreshToken;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class HrCompanyTokenTests
{
    [Theory]
    [InlineData("Human Resources", true)]
    [InlineData("Human Resources", false)]
    [InlineData("Candidate", true)]
    public async Task Login_Should_Include_Assigned_Company_Only_For_Hr(string role, bool assigned)
    {
        var user = CreateUser(assigned);
        using var manager = CreateManager(user, role);
        var tokenService = Substitute.For<ITokenService>();
        tokenService.CreateRefreshToken().Returns("new-refresh");
        var companyRepository = Substitute.For<ICompanyRepository>();
        using var roleManager = new RoleManager<Role>(Substitute.For<IRoleStore<Role>>(), [],
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), NullLogger<RoleManager<Role>>.Instance);
        var handler = new LoginCommandHandler(manager, new AuthBusinessRules(manager, roleManager),
            tokenService, NullLogger<LoginCommandHandler>.Instance, companyRepository);

        await handler.Handle(new() { Email = user.Email!, Password = "Strong1!" }, CancellationToken.None);

        tokenService.Received(1).CreateAccessToken(user.Id, user.Email!, user.Name, user.Surname,
            Arg.Any<IList<string>>(), role == "Human Resources" ? user.CompanyId : null);
        companyRepository.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("Human Resources", true)]
    [InlineData("Human Resources", false)]
    [InlineData("Candidate", true)]
    public async Task Refresh_Should_Include_Current_Assigned_Company_Only_For_Hr(string role, bool assigned)
    {
        var user = CreateUser(assigned);
        using var manager = CreateManager(user, role);
        var tokenService = Substitute.For<ITokenService>();
        tokenService.GetPrincipalFromExpiredToken("expired").Returns(new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())])));
        tokenService.CreateRefreshToken().Returns("new-refresh");
        var handler = new RefreshTokenCommandHandler(tokenService, manager,
            NullLogger<RefreshTokenCommandHandler>.Instance, Substitute.For<ICompanyRepository>());

        await handler.Handle(new() { AccessToken = "expired", RefreshToken = "old-refresh" }, CancellationToken.None);

        tokenService.Received(1).CreateAccessToken(user.Id, user.Email!, user.Name, user.Surname,
            Arg.Any<IList<string>>(), role == "Human Resources" ? user.CompanyId : null);
    }

    private static User CreateUser(bool assigned)
    {
        var user = new User
        {
            Id = Guid.NewGuid(), Email = "hr@example.com", UserName = "hr@example.com",
            Name = "Deniz", Surname = "Öztürk", CompanyId = assigned ? Guid.NewGuid() : null,
        };
        user.RefreshToken = new RefreshToken("old-refresh", user.Id);
        return user;
    }

    private static UserManager<User> CreateManager(User user, string role)
    {
        var store = (IUserStore<User>)Substitute.For(
            [typeof(IUserStore<User>), typeof(IUserEmailStore<User>), typeof(IUserPasswordStore<User>), typeof(IUserRoleStore<User>)], []);
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, "Strong1!");
        store.FindByIdAsync(user.Id.ToString(), Arg.Any<CancellationToken>()).Returns(user);
        ((IUserEmailStore<User>)store).FindByEmailAsync("HR@EXAMPLE.COM", Arg.Any<CancellationToken>()).Returns(user);
        ((IUserPasswordStore<User>)store).GetPasswordHashAsync(user, Arg.Any<CancellationToken>()).Returns(user.PasswordHash);
        ((IUserRoleStore<User>)store).GetRolesAsync(user, Arg.Any<CancellationToken>()).Returns(new List<string> { role });
        return new UserManager<User>(store, Options.Create(new IdentityOptions()), hasher, [], [],
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
    }
}
