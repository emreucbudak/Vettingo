using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Infrastructure.Messages;
using Vettingo.AuthService.Infrastructure.Messaging;

namespace Vettingo.AuthService.UnitTests;

public sealed class JobApplicationCreatedSubscriberTests
{
    [Fact]
    public async Task HandleAsync_Should_Resolve_Email_And_Preserve_Notification_Id()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "candidate@example.com" };
        var store = Substitute.For<IUserStore<User>>();
        store.FindByIdAsync(user.Id.ToString("D"), Arg.Any<CancellationToken>()).Returns(user);
        using var manager = new UserManager<User>(store, Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(), [], [], new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(), null!, NullLogger<UserManager<User>>.Instance);
        var publisher = Substitute.For<IJobApplicationCreatedNotificationPublisher>();
        var subscriber = new JobApplicationCreatedSubscriber(manager, publisher);
        var message = new JobApplicationCreatedMessage { Id = Guid.CreateVersion7(), UserId = user.Id };

        await subscriber.HandleAsync(message, CancellationToken.None);

        await publisher.Received(1).PublishAsync(
            Arg.Is<JobApplicationCreatedNotificationMessage>(notification =>
                notification.Id == message.Id && notification.UserId == user.Id && notification.Email == user.Email),
            CancellationToken.None);
    }
}
