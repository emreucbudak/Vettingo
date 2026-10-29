using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Infrastructure.Messaging;
using Vettingo.AuthService.Infrastructure.Messages;

namespace Vettingo.AuthService.UnitTests;

public sealed class PersonalizedJobPostingNotificationRequestedConsumerTests : IDisposable
{
    private readonly User _user = new() { Id = Guid.NewGuid(), Email = "candidate@example.com" };
    private readonly IUserStore<User> _store = Substitute.For<IUserStore<User>>();
    private readonly IPersonalizedJobPostingNotificationPublisher _publisher =
        Substitute.For<IPersonalizedJobPostingNotificationPublisher>();
    private readonly UserManager<User> _manager;
    private readonly PersonalizedJobPostingNotificationRequestedConsumer _consumer;

    public PersonalizedJobPostingNotificationRequestedConsumerTests()
    {
        _store.FindByIdAsync(_user.Id.ToString("D"), Arg.Any<CancellationToken>()).Returns(_user);
        _publisher.PublishAsync(Arg.Any<PersonalizedJobPostingNotificationDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _manager = new UserManager<User>(_store, Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(), [], [], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
        _consumer = new(_manager, _publisher,
            NullLogger<PersonalizedJobPostingNotificationRequestedConsumer>.Instance);
    }

    [Fact]
    public async Task Handle_Should_Look_Up_User_And_Publish_Email_With_Cancellation()
    {
        var message = new PersonalizedJobPostingNotificationRequestedMessage
        {
            Id = Guid.CreateVersion7(),
            UserId = _user.Id
        };
        using var cancellation = new CancellationTokenSource();

        await _consumer.HandleAsync(message, cancellation.Token);

        await _store.Received(1).FindByIdAsync(_user.Id.ToString("D"), Arg.Any<CancellationToken>());
        await _publisher.Received(1).PublishAsync(
            Arg.Is<PersonalizedJobPostingNotificationDto>(dto =>
                dto.Id == message.Id && dto.UserId == _user.Id && dto.Email == _user.Email),
            cancellation.Token);
    }

    [Fact]
    public async Task Missing_User_Should_Fail_Without_Publishing()
    {
        Func<Task> handle = () => _consumer.HandleAsync(new() { UserId = Guid.NewGuid() }, CancellationToken.None);

        await handle.Should().ThrowAsync<NotFoundException>();
        _publisher.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Missing_Email_Should_Fail_Without_Publishing(string? email)
    {
        _user.Email = email;
        Func<Task> handle = () => _consumer.HandleAsync(new() { UserId = _user.Id }, CancellationToken.None);

        await handle.Should().ThrowAsync<BusinessException>();
        _publisher.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Empty_UserId_Should_Fail_Before_User_Lookup()
    {
        Func<Task> handle = () => _consumer.HandleAsync(new() { UserId = Guid.Empty }, CancellationToken.None);

        await handle.Should().ThrowAsync<BadRequestException>();
        _store.ReceivedCalls().Should().BeEmpty();
        _publisher.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Publish_Failure_Should_Propagate_To_CAP_For_Retry()
    {
        _publisher.PublishAsync(Arg.Any<PersonalizedJobPostingNotificationDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Broker unavailable")));
        Func<Task> handle = () => _consumer.HandleAsync(new() { UserId = _user.Id }, CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>().WithMessage("Broker unavailable");
    }

    public void Dispose() => _manager.Dispose();
}
