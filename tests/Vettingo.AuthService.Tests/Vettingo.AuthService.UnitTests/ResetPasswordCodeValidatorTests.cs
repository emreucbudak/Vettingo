using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordCodeValidator;
using Vettingo.AuthService.Application.Service;

namespace Vettingo.AuthService.UnitTests;

public sealed class ResetPasswordCodeValidatorTests
{
    private readonly MemoryDistributedCache cache = new(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly ITokenService tokens = Substitute.For<ITokenService>();
    private const string Email = "user@example.com";

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("USER@EXAMPLE.COM")]
    public async Task Matching_Code_And_Email_Should_Return_ResetPassword_Token(string email)
    {
        await cache.SetStringAsync(ResetPasswordDefaults.CacheKey(123456), Email);
        tokens.CreateTempToken(Email, "ResetPassword").Returns("reset-password-token");

        var response = await Handler().Handle(new() { Code = "123456", Email = email }, CancellationToken.None);

        response.Token.Should().Be("reset-password-token");
        tokens.Received(1).CreateTempToken(Email, "ResetPassword");
    }

    [Theory]
    [InlineData("654321")]
    [InlineData("123456")]
    public async Task Wrong_Code_Or_Missing_Cache_Should_Not_Issue_Token(string code)
    {
        if (code == "654321")
            await cache.SetStringAsync(ResetPasswordDefaults.CacheKey(123456), Email);
        Func<Task> action = () => Handler().Handle(new() { Code = code, Email = Email }, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
        tokens.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("other@example.com")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Wrong_Or_Missing_Email_Should_Not_Issue_Token(string? email)
    {
        await cache.SetStringAsync(ResetPasswordDefaults.CacheKey(123456), Email);
        Func<Task> action = () => Handler().Handle(new() { Code = "123456", Email = email! }, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
        tokens.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("99999")]
    [InlineData("1000000")]
    [InlineData("0123456")]
    [InlineData("+123456")]
    [InlineData(" 123456")]
    [InlineData("")]
    [InlineData(null)]
    public async Task Invalid_Code_Format_Should_Fail_Before_Cache_Lookup(string? code)
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        Func<Task> action = () => new ResetPasswordCodeValidatorCommandHandler(distributedCache, tokens)
            .Handle(new() { Code = code!, Email = Email }, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedException>();
        distributedCache.ReceivedCalls().Should().BeEmpty();
        tokens.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public void Request_Should_Ignore_Email_In_Json()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var request = JsonSerializer.Deserialize<ResetPasswordCodeValidatorCommandRequest>(
            """{"code":"123456","email":"other@example.com"}""", options)!;

        request.Code.Should().Be("123456");
        request.Email.Should().BeEmpty();
        JsonSerializer.Serialize(request with { Email = Email }, options).Should().NotContain("email");
    }

    private ResetPasswordCodeValidatorCommandHandler Handler() => new(cache, tokens);
}
