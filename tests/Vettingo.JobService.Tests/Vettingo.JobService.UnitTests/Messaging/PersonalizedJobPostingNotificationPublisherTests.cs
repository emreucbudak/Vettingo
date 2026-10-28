using System.Text.Json;
using DotNetCore.CAP;
using FluentAssertions;
using NSubstitute;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Infrastructure.Messaging;

namespace Vettingo.JobService.UnitTests.Messaging;

public sealed class PersonalizedJobPostingNotificationPublisherTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Publish_Should_Send_Only_Notification_And_User_Ids_To_Auth(bool hasId)
    {
        var cap = Substitute.For<ICapPublisher>();
        cap.PublishAsync(Arg.Any<string>(), Arg.Any<PersonalizedJobPostingNotificationDto>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var publisher = new PersonalizedJobPostingNotificationPublisher(cap);
        var notification = new PersonalizedJobPostingNotificationDto
        {
            Id = hasId ? Guid.CreateVersion7() : Guid.Empty,
            UserId = Guid.NewGuid()
        };
        using var cancellation = new CancellationTokenSource();

        await publisher.PublishAsync(notification, cancellation.Token);

        var arguments = cap.ReceivedCalls().Single().GetArguments();
        arguments[0].Should().Be("vettingo.auth.personalized-job-posting.notification.requested.v1");
        arguments[3].Should().Be(cancellation.Token);
        var published = (PersonalizedJobPostingNotificationDto)arguments[1]!;
        published.Id.Should().NotBeEmpty();
        published.UserId.Should().Be(notification.UserId);
        if (hasId)
        {
            published.Id.Should().Be(notification.Id);
        }

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(published, JsonSerializerOptions.Web));
        document.RootElement.EnumerateObject().Select(property => property.Name)
            .Should().BeEquivalentTo("id", "userId");
    }
}
