using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerRegister;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class EmployerRegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateEmployerCompanyWithSubscriberId()
    {
        Guid token = Guid.NewGuid();
        Guid subscriberId = Guid.NewGuid();
        const string password = "Strong1!";
        IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();
        string passwordHash = passwordHasher.HashPassword(new User(), password);
        string registrationJson = JsonSerializer.Serialize(new
        {
            Name = "Emre",
            Surname = "Üçbudak",
            Email = "employer@example.com",
            PasswordHash = passwordHash,
            Role = "Company",
            CompanyName = "Vettingo"
        });

        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache
            .GetAsync(token.ToString("D"), Arg.Any<CancellationToken>())
            .Returns(Encoding.UTF8.GetBytes(registrationJson));
        cache
            .RemoveAsync(token.ToString("D"), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        IUserStore<User> userStore =
            Substitute.For<IUserStore<User>, IUserEmailStore<User>, IUserRoleStore<User>>();
        IUserEmailStore<User> emailStore = (IUserEmailStore<User>)userStore;
        IUserRoleStore<User> userRoleStore = (IUserRoleStore<User>)userStore;
        User? createdUser = null;

        emailStore
            .FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User?)null);
        userStore
            .CreateAsync(
                Arg.Do<User>(user => createdUser = user),
                Arg.Any<CancellationToken>())
            .Returns(IdentityResult.Success);
        userStore
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(IdentityResult.Success);
        userRoleStore
            .IsInRoleAsync(
                Arg.Any<User>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(false);
        userRoleStore
            .AddToRoleAsync(
                Arg.Any<User>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        ILookupNormalizer normalizer = new UpperInvariantLookupNormalizer();
        IdentityErrorDescriber errorDescriber = new();
        UserManager<User> userManager = new(
            userStore,
            Options.Create(new IdentityOptions()),
            passwordHasher,
            [],
            [],
            normalizer,
            errorDescriber,
            null!,
            NullLogger<UserManager<User>>.Instance);

        IRoleStore<Role> roleStore = Substitute.For<IRoleStore<Role>>();
        roleStore
            .FindByNameAsync("COMPANY", Arg.Any<CancellationToken>())
            .Returns(new Role { Name = "Company", NormalizedName = "COMPANY" });
        RoleManager<Role> roleManager = new(
            roleStore,
            [],
            normalizer,
            errorDescriber,
            NullLogger<RoleManager<Role>>.Instance);

        ICompanyRepository companyRepository = Substitute.For<ICompanyRepository>();
        Company? createdCompany = null;
        companyRepository
            .GetCompanyByIdAsync(subscriberId)
            .Returns((Company?)null);
        companyRepository
            .AddCompanyAsync(Arg.Do<Company>(company => createdCompany = company))
            .Returns(Task.CompletedTask);
        companyRepository.SaveChangesAsync().Returns(1);

        EmployerRegisterCommandHandler handler = new(
            cache,
            userManager,
            new AuthBusinessRules(userManager, roleManager),
            companyRepository,
            NullLogger<EmployerRegisterCommandHandler>.Instance);

        await handler.Handle(
            new EmployerRegisterCommandRequest
            {
                Token = token,
                SubscriberId = subscriberId
            },
            CancellationToken.None);

        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be("employer@example.com");
        passwordHasher
            .VerifyHashedPassword(createdUser, createdUser.PasswordHash!, password)
            .Should()
            .Be(PasswordVerificationResult.Success);
        await userRoleStore.Received(1).AddToRoleAsync(
            createdUser,
            "COMPANY",
            Arg.Any<CancellationToken>());

        createdCompany.Should().NotBeNull();
        createdCompany!.Id.Should().Be(subscriberId);
        createdCompany.CompanyName.Should().Be("Vettingo");
        createdCompany.CompanyEmail.Should().Be("employer@example.com");

        await cache.Received(1).RemoveAsync(
            token.ToString("D"),
            Arg.Any<CancellationToken>());
    }
}
