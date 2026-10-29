using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.ResetPasswordTemp;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Application.Service;

namespace Vettingo.AuthService.UnitTests;

public sealed class ResetPasswordTempTests
{
    private readonly MemoryDistributedCache cache = new(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly IResetPasswordCodeSenderPublisher publisher = Substitute.For<IResetPasswordCodeSenderPublisher>();
    private const string Email = "user@example.com";

    [Fact]
    public async Task Handle_Should_Cache_Email_Before_Publishing_Six_Digit_Code()
    {
        using var cancellation = new CancellationTokenSource();
        publisher.CodeSend(Arg.Any<CodeMessage>(), cancellation.Token).Returns(async call =>
        {
            var message = call.Arg<CodeMessage>();
            message.Email.Should().Be(Email);
            int code = int.Parse(message.Code);
            code.Should().BeInRange(100000, 999999);
            (await cache.GetStringAsync(ResetPasswordDefaults.CacheKey(code))).Should().Be(Email);
        });

        await new ResetPasswordTempCommandHandler(cache, publisher)
            .Handle(new() { Email = Email }, cancellation.Token);

        await publisher.Received(1).CodeSend(Arg.Any<CodeMessage>(), cancellation.Token);
    }

    [Fact]
    public async Task Handle_Should_Expire_Cached_Email_After_Five_Minutes()
    {
        var distributedCache = Substitute.For<IDistributedCache>();

        await new ResetPasswordTempCommandHandler(distributedCache, publisher)
            .Handle(new() { Email = Email }, CancellationToken.None);

        int code = int.Parse(SentMessage().Code);
        await distributedCache.Received(1).SetAsync(ResetPasswordDefaults.CacheKey(code),
            Arg.Is<byte[]>(bytes => Encoding.UTF8.GetString(bytes) == Email),
            Arg.Is<DistributedCacheEntryOptions>(options => options.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(5)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Publish_Failure_Should_Remove_Cached_Email()
    {
        publisher.CodeSend(Arg.Any<CodeMessage>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Broker unavailable")));
        Func<Task> action = () => new ResetPasswordTempCommandHandler(cache, publisher)
            .Handle(new() { Email = Email }, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>();

        int code = int.Parse(SentMessage().Code);
        (await cache.GetStringAsync(ResetPasswordDefaults.CacheKey(code))).Should().BeNull();
    }

    private CodeMessage SentMessage() => (CodeMessage)publisher.ReceivedCalls().Single().GetArguments()[0]!;
}
