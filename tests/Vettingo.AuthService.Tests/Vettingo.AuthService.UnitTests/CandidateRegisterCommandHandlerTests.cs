using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.CandidateRegister;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class CandidateRegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateCandidateWithSubscriberIdAndRemoveTemporaryRegistration()
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
            Email = "emre@example.com",
            PasswordHash = passwordHash,
            Role = "Candidate"
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
            .FindByIdAsync(
                subscriberId.ToString("D"),
                Arg.Any<CancellationToken>())
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
            .FindByNameAsync("CANDIDATE", Arg.Any<CancellationToken>())
            .Returns(new Role { Name = "Candidate", NormalizedName = "CANDIDATE" });
        RoleManager<Role> roleManager = new(
            roleStore,
            [],
            normalizer,
            errorDescriber,
            NullLogger<RoleManager<Role>>.Instance);

        CandidateRegisterCommandHandler handler = new(
            cache,
            userManager,
            new AuthBusinessRules(userManager, roleManager),
            NullLogger<CandidateRegisterCommandHandler>.Instance);

        await handler.Handle(
            new CandidateRegisterCommandRequest
            {
                Token = token,
                SubscriberId = subscriberId
            },
            CancellationToken.None);

        createdUser.Should().NotBeNull();
        createdUser!.Id.Should().Be(subscriberId);
        createdUser.Email.Should().Be("emre@example.com");
        passwordHasher
            .VerifyHashedPassword(createdUser, createdUser.PasswordHash!, password)
            .Should()
            .Be(PasswordVerificationResult.Success);
        await userRoleStore.Received(1).AddToRoleAsync(
            createdUser,
            "CANDIDATE",
            Arg.Any<CancellationToken>());
        await cache.Received(1).RemoveAsync(
            token.ToString("D"),
            Arg.Any<CancellationToken>());
    }
}
