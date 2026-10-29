using System.Text;
using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TempRegister;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Application.Validations;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class RegisterTests : IDisposable
{
    private readonly IUserStore<User> store = (IUserStore<User>)Substitute.For(
        [typeof(IUserStore<User>), typeof(IUserEmailStore<User>), typeof(IUserPasswordStore<User>), typeof(IUserRoleStore<User>)], []);
    private readonly MemoryDistributedCache cache = new(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly ITokenService tokens = Substitute.For<ITokenService>();
    private readonly IRegisterConfirmCodeSenderPublisher publisher = Substitute.For<IRegisterConfirmCodeSenderPublisher>();
    private readonly UserManager<User> manager;
    private readonly RoleManager<Role> roles;
    private readonly AuthBusinessRules auth;
    private readonly TempRegisterCommandRequestValidator validator = new(Options.Create(new IdentityOptions()));

    public RegisterTests()
    {
        manager = new UserManager<User>(store, Options.Create(new IdentityOptions()), new PasswordHasher<User>(),
            [], [new PasswordValidator<User>()], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
        var roleStore = Substitute.For<IRoleStore<Role>>();
        foreach (string role in new[] { "Candidate", "Company", "Human Resources" })
            roleStore.FindByNameAsync(role.ToUpperInvariant(), Arg.Any<CancellationToken>()).Returns(new Role { Name = role });
        roles = new RoleManager<Role>(roleStore, [], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), NullLogger<RoleManager<Role>>.Instance);
        auth = new AuthBusinessRules(manager, roles);
        tokens.CreateTempToken("user@example.com", "Register").Returns("register-token");
        tokens.CreateRefreshToken().Returns("refresh-token");
        store.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
        store.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
        store.DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(IdentityResult.Success);
    }

    [Theory]
    [InlineData("Candidate")]
    [InlineData("Company")]
    [InlineData("Human Resources")]
    public async Task Registration_Should_Persist_Identity_Hash_And_Cached_Role_Only_After_Email_Verification(string role)
    {
        var response = await Temp().Handle(Request() with { Role = role }, CancellationToken.None);
        response.TempToken.Should().Be("register-token");
        await store.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        int code = SentCode();
        code.Should().BeInRange(100000, 999999);
        var data = JsonSerializer.Deserialize<TempRegisterData>((await cache.GetStringAsync(TempRegisterDefaults.CacheKey(code)))!)!;
        data.Email.Should().Be("user@example.com");
        data.Role.Should().Be(role);
        data.PasswordHash.Should().NotBe(Request().Password);
        manager.PasswordHasher.VerifyHashedPassword(new User(), data.PasswordHash, Request().Password)
            .Should().Be(PasswordVerificationResult.Success);

        await Register().Handle(new() { Code = code, Email = "USER@EXAMPLE.COM" }, CancellationToken.None);

        var user = (User)store.ReceivedCalls().Single(call => call.GetMethodInfo().Name == "CreateAsync").GetArguments()[0]!;
        user.Email.Should().Be(data.Email);
        user.EmailConfirmed.Should().BeTrue();
        user.PasswordHash.Should().Be(data.PasswordHash);
        user.Id.Should().NotBeEmpty();
        await ((IUserRoleStore<User>)store).Received(1).AddToRoleAsync(user, role.ToUpperInvariant(), Arg.Any<CancellationToken>());
        (await cache.GetStringAsync(TempRegisterDefaults.CacheKey(code))).Should().BeNull();

        Func<Task> replay = () => Register().Handle(new() { Code = code, Email = data.Email }, CancellationToken.None);
        await replay.Should().ThrowAsync<UnauthorizedException>();
        await store.Received(1).CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Existing_Email_Should_Fail_Before_Caching_Or_Sending_Code()
    {
        ((IUserEmailStore<User>)store).FindByEmailAsync("USER@EXAMPLE.COM", Arg.Any<CancellationToken>())
            .Returns(new User { Email = Request().Email });
        var distributedCache = Substitute.For<IDistributedCache>();
        Func<Task> action = () => Temp(distributedCache).Handle(Request(), CancellationToken.None);

        await action.Should().ThrowAsync<BusinessException>().WithMessage("Bu e-posta adresi zaten kayıtlı");
        distributedCache.ReceivedCalls().Should().BeEmpty();
        publisher.ReceivedCalls().Should().BeEmpty();
        tokens.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("invalid", "Strong1!", "Candidate")]
    [InlineData("user@example.com", "weak", "Candidate")]
    [InlineData("user@example.com", "Strong1!", "Admin")]
    [InlineData("user@example.com", "Strong1!", "TempRegister")]
    public async Task Invalid_Request_Should_Not_Create_Challenge(string email, string password, string role)
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        Func<Task> action = () => Temp(distributedCache).Handle(new() { Email = email, Password = password, Role = role }, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
        store.ReceivedCalls().Should().BeEmpty();
        distributedCache.ReceivedCalls().Should().BeEmpty();
        publisher.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Challenge_Should_Expire_After_Five_Minutes_And_Contain_No_Plain_Password()
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        await Temp(distributedCache).Handle(Request(), CancellationToken.None);

        await distributedCache.Received(1).SetAsync(TempRegisterDefaults.CacheKey(SentCode()),
            Arg.Is<byte[]>(bytes => !Encoding.UTF8.GetString(bytes).Contains(Request().Password)),
            Arg.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Publish_Failure_Should_Remove_Challenge()
    {
        publisher.CodeSend(Arg.Any<CodeMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Broker unavailable")));
        Func<Task> action = () => Temp().Handle(Request(), CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();
        (await cache.GetStringAsync(TempRegisterDefaults.CacheKey(SentCode()))).Should().BeNull();
    }

    [Fact]
    public async Task Missing_Or_Expired_Challenge_Should_Not_Create_User()
    {
        Func<Task> action = () => Register().Handle(new() { Code = 123456, Email = Request().Email }, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
        await store.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Different_Token_Email_Should_Not_Create_User_Or_Consume_Challenge()
    {
        await Temp().Handle(Request(), CancellationToken.None);
        int code = SentCode();
        Func<Task> action = () => Register().Handle(new() { Code = code, Email = "other@example.com" }, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
        await store.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        (await cache.GetStringAsync(TempRegisterDefaults.CacheKey(code))).Should().NotBeNull();
    }

    [Fact]
    public async Task Email_Registered_After_Challenge_Should_Not_Create_Another_User()
    {
        await Temp().Handle(Request(), CancellationToken.None);
        ((IUserEmailStore<User>)store).FindByEmailAsync("USER@EXAMPLE.COM", Arg.Any<CancellationToken>())
            .Returns(new User { Email = Request().Email });
        Func<Task> action = () => Register().Handle(new() { Code = SentCode(), Email = Request().Email }, CancellationToken.None);

        await action.Should().ThrowAsync<BusinessException>();
        await store.DidNotReceive().CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task User_Creation_Failure_Should_Not_Consume_Challenge_Or_Assign_Role()
    {
        await Temp().Handle(Request(), CancellationToken.None);
        store.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Kayıt oluşturulamadı." }));
        Func<Task> action = () => Register().Handle(new() { Code = SentCode(), Email = Request().Email }, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequestException>();
        await ((IUserRoleStore<User>)store).DidNotReceiveWithAnyArgs().AddToRoleAsync(default!, default!, default);
        (await cache.GetStringAsync(TempRegisterDefaults.CacheKey(SentCode()))).Should().NotBeNull();
    }

    [Fact]
    public async Task Role_Assignment_Failure_Should_Remove_Incomplete_User()
    {
        await Temp().Handle(Request(), CancellationToken.None);
        store.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(IdentityResult.Failed(new IdentityError { Description = "Rol atanamadı." }));
        Func<Task> action = () => Register().Handle(new() { Code = SentCode(), Email = Request().Email }, CancellationToken.None);

        await action.Should().ThrowAsync<BadRequestException>();
        await store.Received(1).DeleteAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        (await cache.GetStringAsync(TempRegisterDefaults.CacheKey(SentCode()))).Should().NotBeNull();
    }

    private static TempRegisterCommandRequest Request() => new() { Email = "user@example.com", Password = "Strong1!", Role = "Candidate" };
    private int SentCode() => int.Parse(((CodeMessage)publisher.ReceivedCalls().Single().GetArguments()[0]!).Code);
    private TempRegisterCommandHandler Temp(IDistributedCache? distributedCache = null) => new(manager, auth, tokens,
        distributedCache ?? cache, publisher, validator);
    private RegisterCommandHandler Register() => new(manager, auth, tokens, cache, new RegisterCommandRequestValidator());

    public void Dispose()
    {
        manager.Dispose();
        roles.Dispose();
    }
}
