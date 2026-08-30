using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.EmployerTempRegister;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests
{
    public class EmployerTempRegisterCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldHashPasswordAndCacheRegistrationForFiveMinutes()
        {
            IDistributedCache cache = Substitute.For<IDistributedCache>();
            string? storedKey = null;
            byte[]? storedValue = null;
            DistributedCacheEntryOptions? storedOptions = null;

            cache.SetAsync(
                    Arg.Do<string>(value => storedKey = value),
                    Arg.Do<byte[]>(value => storedValue = value),
                    Arg.Do<DistributedCacheEntryOptions>(value => storedOptions = value),
                    Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            ILookupNormalizer normalizer = new UpperInvariantLookupNormalizer();
            IdentityErrorDescriber errorDescriber = new();
            IUserEmailStore<User> userStore = Substitute.For<IUserEmailStore<User>>();
            userStore
                .FindByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns((User?)null);

            UserManager<User> userManager = new(
                userStore,
                Options.Create(new IdentityOptions()),
                passwordHasher,
                [],
                [new PasswordValidator<User>()],
                normalizer,
                errorDescriber,
                null!,
                NullLogger<UserManager<User>>.Instance);

            RoleManager<Role> roleManager = new(
                Substitute.For<IRoleStore<Role>>(),
                [],
                normalizer,
                errorDescriber,
                NullLogger<RoleManager<Role>>.Instance);

            AuthBusinessRules businessRules = new(userManager, roleManager);
            EmployerTempRegisterCommandHandler handler = new(
                cache,
                userManager,
                businessRules,
                NullLogger<EmployerTempRegisterCommandHandler>.Instance);

            EmployerTempRegisterCommandRequest request = new()
            {
                Name = "Emre",
                Surname = "Üçbudak",
                Email = "emre@example.com",
                Password = "Strong1!",
                CompanyName = "Vettingo",
                CompanyDescription = "Hiring platform",
                CompanyPhone = "+905551112233",
                CompanyEmail = "company@example.com",
                CompanyAddress = "İstanbul"
            };

            EmployerTempRegisterCommandResponse response = await handler.Handle(
                request,
                CancellationToken.None);

            response.Token.Should().NotBeEmpty();
            storedKey.Should().Be(response.Token.ToString("D"));
            storedOptions.Should().NotBeNull();
            storedOptions!.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(5));
            storedValue.Should().NotBeNull();

            string serializedRegistration = Encoding.UTF8.GetString(storedValue!);
            serializedRegistration.Should().NotContain(request.Password);

            using JsonDocument registrationData = JsonDocument.Parse(serializedRegistration);
            JsonElement registration = registrationData.RootElement;
            string passwordHash = registration.GetProperty("PasswordHash").GetString()!;

            registration.GetProperty("Email").GetString().Should().Be(request.Email);
            registration.GetProperty("CompanyName").GetString().Should().Be(request.CompanyName);
            registration.GetProperty("Role").GetString().Should().Be("Company");
            passwordHash.Should().NotBe(request.Password);
            passwordHasher
                .VerifyHashedPassword(new User(), passwordHash, request.Password)
                .Should()
                .Be(PasswordVerificationResult.Success);
        }
    }
}
