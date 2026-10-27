using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Exceptions;
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
    [InlineData("Company", true)]
    [InlineData("Company", false)]
    [InlineData("Candidate", true)]
    public async Task Login_Should_Include_Company_Id_And_Name_Only_For_Company_And_Assigned_Hr(string role, bool assigned)
    {
        var user = CreateUser(assigned);
        using var manager = CreateManager(user, role);
        var tokenService = Substitute.For<ITokenService>();
        tokenService.CreateRefreshToken().Returns("new-refresh");
        var companyRepository = Substitute.For<ICompanyRepository>();
        var company = ConfigureCompany(companyRepository, user, role);
        using var roleManager = new RoleManager<Role>(Substitute.For<IRoleStore<Role>>(), [],
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), NullLogger<RoleManager<Role>>.Instance);
        var handler = new LoginCommandHandler(manager, new AuthBusinessRules(manager, roleManager),
            tokenService, NullLogger<LoginCommandHandler>.Instance, companyRepository);

        await handler.Handle(new() { Email = user.Email!, Password = "Strong1!" }, CancellationToken.None);

        tokenService.Received(1).CreateAccessToken(user.Id, user.Email!, user.Name, user.Surname,
            Arg.Any<IList<string>>(), company?.Id, company?.CompanyName);
        if (company is null) companyRepository.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("Human Resources", true)]
    [InlineData("Human Resources", false)]
    [InlineData("Company", true)]
    [InlineData("Company", false)]
    [InlineData("Candidate", true)]
    public async Task Refresh_Should_Include_Company_Id_And_Name_Only_For_Company_And_Assigned_Hr(string role, bool assigned)
    {
        var user = CreateUser(assigned);
        using var manager = CreateManager(user, role);
        var tokenService = Substitute.For<ITokenService>();
        tokenService.GetPrincipalFromExpiredToken("expired").Returns(new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())])));
        tokenService.CreateRefreshToken().Returns("new-refresh");
        var companyRepository = Substitute.For<ICompanyRepository>();
        var company = ConfigureCompany(companyRepository, user, role);
        var handler = new RefreshTokenCommandHandler(tokenService, manager,
            NullLogger<RefreshTokenCommandHandler>.Instance, companyRepository);

        await handler.Handle(new() { AccessToken = "expired", RefreshToken = "old-refresh" }, CancellationToken.None);

        tokenService.Received(1).CreateAccessToken(user.Id, user.Email!, user.Name, user.Surname,
            Arg.Any<IList<string>>(), company?.Id, company?.CompanyName);
        if (company is null) companyRepository.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("Company", false)]
    [InlineData("Company", true)]
    [InlineData("Human Resources", false)]
    [InlineData("Human Resources", true)]
    public async Task Missing_Company_Should_Not_Issue_Tokens(string role, bool refresh)
    {
        var user = CreateUser(true);
        using var manager = CreateManager(user, role);
        var tokenService = Substitute.For<ITokenService>();
        var companyRepository = Substitute.For<ICompanyRepository>();
        Func<Task> issueToken;
        if (refresh)
        {
            tokenService.GetPrincipalFromExpiredToken("expired").Returns(new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())])));
            var handler = new RefreshTokenCommandHandler(tokenService, manager,
                NullLogger<RefreshTokenCommandHandler>.Instance, companyRepository);
            issueToken = () => handler.Handle(new() { AccessToken = "expired", RefreshToken = "old-refresh" }, CancellationToken.None);
        }
        else
        {
            using var roleManager = new RoleManager<Role>(Substitute.For<IRoleStore<Role>>(), [],
                new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), NullLogger<RoleManager<Role>>.Instance);
            var handler = new LoginCommandHandler(manager, new AuthBusinessRules(manager, roleManager),
                tokenService, NullLogger<LoginCommandHandler>.Instance, companyRepository);
            issueToken = () => handler.Handle(new() { Email = user.Email!, Password = "Strong1!" }, CancellationToken.None);
        }

        await issueToken.Should().ThrowAsync<UnauthorizedException>().WithMessage("Şirket hesabı bulunamadı.");
        tokenService.DidNotReceiveWithAnyArgs().CreateAccessToken(default, default!, default!, default!, default!, default, default);
        tokenService.DidNotReceive().CreateRefreshToken();
    }

    private static Company? ConfigureCompany(ICompanyRepository repository, User user, string role)
    {
        var company = new Company();
        if (role == "Company")
        {
            company.RegisterCompany(Guid.NewGuid(), "Vettingo Teknoloji");
            repository.GetCompanyByEmailAsync(user.Email!).Returns(company);
        }
        else if (role == "Human Resources" && user.CompanyId.HasValue)
        {
            company.RegisterCompany(user.CompanyId.Value, "Vettingo Teknoloji");
            repository.GetCompanyByIdAsync(user.CompanyId.Value).Returns(company);
        }
        else
        {
            return null;
        }
        return company;
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
