using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Login;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.TwoFactorLogin;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Application.Rules;
using Vettingo.AuthService.Application.Service;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class TwoFactorLoginTests : IDisposable
{
    private readonly User _user = new()
    {
        Id = Guid.NewGuid(), Email = "user@example.com", UserName = "user@example.com",
        Name = "Deniz", Surname = "Öztürk", IsTwoFactorAuthenticated = true
    };
    private readonly IUserStore<User> _store = (IUserStore<User>)Substitute.For(
        [typeof(IUserStore<User>), typeof(IUserEmailStore<User>), typeof(IUserPasswordStore<User>), typeof(IUserRoleStore<User>)], []);
    private readonly MemoryDistributedCache _cache = new(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();
    private readonly ILoginCodeSenderPublisher _publisher = Substitute.For<ILoginCodeSenderPublisher>();
    private readonly ICompanyRepository _companies = Substitute.For<ICompanyRepository>();
    private readonly UserManager<User> _manager;
    private readonly RoleManager<Role> _roleManager;

    public TwoFactorLoginTests()
    {
        var hasher = new PasswordHasher<User>();
        _user.PasswordHash = hasher.HashPassword(_user, "Strong1!");
        ((IUserEmailStore<User>)_store).FindByEmailAsync("USER@EXAMPLE.COM", Arg.Any<CancellationToken>()).Returns(_user);
        ((IUserPasswordStore<User>)_store).GetPasswordHashAsync(_user, Arg.Any<CancellationToken>()).Returns(_user.PasswordHash);
        ((IUserRoleStore<User>)_store).GetRolesAsync(_user, Arg.Any<CancellationToken>()).Returns(new List<string> { "Candidate" });
        _manager = new UserManager<User>(_store, Options.Create(new IdentityOptions()), hasher, [], [],
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
        _roleManager = new RoleManager<Role>(Substitute.For<IRoleStore<Role>>(), [],
            new UpperInvariantLookupNormalizer(), new IdentityErrorDescriber(), NullLogger<RoleManager<Role>>.Instance);
        _tokens.CreateTempToken(_user.Email!).Returns("temp-token");
        _tokens.CreateAccessToken(_user.Id, _user.Email!, _user.Name, _user.Surname,
            Arg.Any<IList<string>>(), Arg.Any<Guid?>(), Arg.Any<string?>()).Returns("access-token");
        _tokens.CreateRefreshToken().Returns("refresh-token");
    }

    [Fact]
    public async Task Disabled_TwoFactor_Should_Issue_Tokens_Without_Email_Or_Cache()
    {
        _user.IsTwoFactorAuthenticated = false;
        var cache = Substitute.For<IDistributedCache>();

        var response = await Login(cache).Handle(new() { Email = _user.Email!, Password = "Strong1!" }, CancellationToken.None);

        response.AccessToken.Should().Be("access-token");
        response.RefreshToken.Should().Be("refresh-token");
        response.TempToken.Should().BeNull();
        _tokens.DidNotReceiveWithAnyArgs().CreateTempToken(default!);
        _publisher.ReceivedCalls().Should().BeEmpty();
        cache.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Enabled_TwoFactor_Should_Send_Code_And_Return_Only_TempToken()
    {
        using var cancellation = new CancellationTokenSource();
        var response = await Login().Handle(new() { Email = _user.Email!, Password = "Strong1!" }, cancellation.Token);

        response.AccessToken.Should().BeNull();
        response.RefreshToken.Should().BeNull();
        response.TempToken.Should().Be("temp-token");
        var message = SentCode();
        message.Email.Should().Be(_user.Email);
        int code = int.Parse(message.Code);
        code.Should().BeInRange(100000, 999999);
        (await _cache.GetStringAsync(TwoFactorLoginDefaults.CacheKey(code))).Should().Be(_user.Email);
        _tokens.Received(1).CreateTempToken(_user.Email!);
        await _publisher.Received(1).CodeSend(message, cancellation.Token);
        AssertNoAccessTokens();
    }

    [Fact]
    public async Task Challenge_Cache_Should_Expire_After_Five_Minutes()
    {
        var cache = Substitute.For<IDistributedCache>();

        await Login(cache).Handle(new() { Email = _user.Email!, Password = "Strong1!" }, CancellationToken.None);

        await cache.Received(1).SetAsync(
            TwoFactorLoginDefaults.CacheKey(int.Parse(SentCode().Code)),
            Arg.Is<byte[]>(bytes => Encoding.UTF8.GetString(bytes) == _user.Email),
            Arg.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
            CancellationToken.None);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Wrong_Password_Should_Not_Create_Challenge_Or_Tokens(bool enabled)
    {
        _user.IsTwoFactorAuthenticated = enabled;
        var cache = Substitute.For<IDistributedCache>();
        Func<Task> login = () => Login(cache).Handle(new() { Email = _user.Email!, Password = "Wrong1!" }, CancellationToken.None);

        await login.Should().ThrowAsync<UnauthorizedException>();

        _tokens.ReceivedCalls().Should().BeEmpty();
        _publisher.ReceivedCalls().Should().BeEmpty();
        cache.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Publish_Failure_Should_Remove_Challenge()
    {
        _publisher.CodeSend(Arg.Any<CodeMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Broker unavailable")));
        Func<Task> login = () => Login().Handle(new() { Email = _user.Email!, Password = "Strong1!" }, CancellationToken.None);

        await login.Should().ThrowAsync<InvalidOperationException>();

        (await _cache.GetStringAsync(TwoFactorLoginDefaults.CacheKey(int.Parse(SentCode().Code)))).Should().BeNull();
        AssertNoAccessTokens();
    }

    [Theory]
    [InlineData("Candidate", false)]
    [InlineData("Company", false)]
    [InlineData("Human Resources", true)]
    [InlineData("Human Resources", false)]
    public async Task Valid_Code_Should_Issue_Tokens_With_Current_Roles_And_Company(string role, bool assigned)
    {
        await _cache.SetStringAsync(TwoFactorLoginDefaults.CacheKey(123456), _user.Email!);
        _user.CompanyId = assigned ? Guid.NewGuid() : null;
        ((IUserRoleStore<User>)_store).GetRolesAsync(_user, Arg.Any<CancellationToken>()).Returns(new List<string> { role });
        Company? company = null;
        if (role == "Company" || assigned)
        {
            company = new Company();
            company.RegisterCompany(_user.CompanyId ?? Guid.NewGuid(), "Vettingo Teknoloji");
            _companies.GetCompanyByEmailAsync(_user.Email!).Returns(company);
            _companies.GetCompanyByIdAsync(company.Id).Returns(company);
        }

        var response = await Verify().Handle(new() { Code = 123456, Email = _user.Email!.ToUpperInvariant() }, CancellationToken.None);

        response.AccessToken.Should().Be("access-token");
        response.RefreshToken.Should().Be("refresh-token");
        _tokens.Received(1).CreateAccessToken(_user.Id, _user.Email!, _user.Name, _user.Surname,
            Arg.Is<IList<string>>(roles => roles.Count == 1 && roles[0] == role), company?.Id, company?.CompanyName);
        (await _cache.GetStringAsync(TwoFactorLoginDefaults.CacheKey(123456))).Should().BeNull();
    }

    [Theory]
    [InlineData(654321)]
    [InlineData(99999)]
    [InlineData(1000000)]
    public async Task Wrong_Code_Should_Fail_Without_Issuing_Tokens(int code)
    {
        await _cache.SetStringAsync(TwoFactorLoginDefaults.CacheKey(123456), _user.Email!);
        Func<Task> verify = () => Verify().Handle(new() { Code = code, Email = _user.Email! }, CancellationToken.None);

        await verify.Should().ThrowAsync<UnauthorizedException>();

        AssertNoAccessTokens();
    }

    [Fact]
    public async Task Missing_Or_Expired_Cache_Entry_Should_Fail()
    {
        Func<Task> verify = () => Verify().Handle(new() { Code = 123456, Email = _user.Email! }, CancellationToken.None);

        await verify.Should().ThrowAsync<UnauthorizedException>();

        AssertNoAccessTokens();
    }

    [Fact]
    public async Task Successful_Code_Should_Not_Be_Reusable()
    {
        await _cache.SetStringAsync(TwoFactorLoginDefaults.CacheKey(123456), _user.Email!);
        var request = new TwoFactorLoginCommandRequest { Code = 123456, Email = _user.Email! };
        await Verify().Handle(request, CancellationToken.None);
        _tokens.ClearReceivedCalls();

        Func<Task> verify = () => Verify().Handle(request, CancellationToken.None);
        await verify.Should().ThrowAsync<UnauthorizedException>();

        AssertNoAccessTokens();
    }

    [Fact]
    public async Task Deleted_User_Should_Not_Get_Tokens()
    {
        await _cache.SetStringAsync(TwoFactorLoginDefaults.CacheKey(123456), "deleted@example.com");
        Func<Task> verify = () => Verify().Handle(new() { Code = 123456, Email = "deleted@example.com" }, CancellationToken.None);

        await verify.Should().ThrowAsync<UnauthorizedException>();

        AssertNoAccessTokens();
    }

    [Fact]
    public async Task Missing_Company_Should_Not_Get_Tokens()
    {
        await _cache.SetStringAsync(TwoFactorLoginDefaults.CacheKey(123456), _user.Email!);
        ((IUserRoleStore<User>)_store).GetRolesAsync(_user, Arg.Any<CancellationToken>()).Returns(new List<string> { "Company" });
        Func<Task> verify = () => Verify().Handle(new() { Code = 123456, Email = _user.Email! }, CancellationToken.None);

        await verify.Should().ThrowAsync<UnauthorizedException>().WithMessage("Şirket hesabı bulunamadı.");

        AssertNoAccessTokens();
    }

    [Theory]
    [InlineData("other@example.com")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Wrong_Or_Missing_Email_Should_Not_Issue_Tokens_Or_Consume_Code(string? email)
    {
        string cacheKey = TwoFactorLoginDefaults.CacheKey(123456);
        await _cache.SetStringAsync(cacheKey, _user.Email!);
        _store.ClearReceivedCalls();
        Func<Task> verify = () => Verify().Handle(new() { Code = 123456, Email = email! }, CancellationToken.None);

        await verify.Should().ThrowAsync<UnauthorizedException>();

        AssertNoAccessTokens();
        await ((IUserEmailStore<User>)_store).DidNotReceiveWithAnyArgs().FindByEmailAsync(default!, default);
        (await _cache.GetStringAsync(cacheKey)).Should().Be(_user.Email);
    }

    private LoginCommandHandler Login(IDistributedCache? cache = null) => new(_manager,
        new AuthBusinessRules(_manager, _roleManager), _tokens, NullLogger<LoginCommandHandler>.Instance,
        _companies, cache ?? _cache, _publisher);

    private TwoFactorLoginCommandHandler Verify() => new(_manager, _tokens, _companies, _cache);

    private CodeMessage SentCode() => (CodeMessage)_publisher.ReceivedCalls().Single().GetArguments()[0]!;

    private void AssertNoAccessTokens()
    {
        _tokens.DidNotReceiveWithAnyArgs().CreateAccessToken(default, default!, default!, default!, default!, default, default);
        _tokens.DidNotReceive().CreateRefreshToken();
    }

    public void Dispose()
    {
        _manager.Dispose();
        _roleManager.Dispose();
    }
}
