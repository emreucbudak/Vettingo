using System.Reflection;
using System.Text.Json;
using DotNetCore.CAP;
using FluentAssertions;
using NSubstitute;
using Vettingo.NotificationService.Infrastructure.Messaging;
using AuthConsumer = Vettingo.AuthService.Infrastructure.Messaging.PersonalizedJobPostingNotificationRequestedConsumer;
using AuthRequest = Vettingo.AuthService.Infrastructure.Messages.PersonalizedJobPostingNotificationRequestedMessage;
using AuthPublisher = Vettingo.AuthService.Infrastructure.Messaging.PersonalizedJobPostingNotificationPublisher;
using EvaluationPublisher = Vettingo.EvaluationService.Application.Messaging.PersonalizedJobPostingNotificationPublisher;
using EvaluationDto = Vettingo.EvaluationService.Application.DTOs.PersonalizedJobPostingNotificationDto;
using PublishedDto = Vettingo.AuthService.Application.DTOs.PersonalizedJobPostingNotificationDto;
using ReceivedDto = Vettingo.NotificationService.Application.DTOs.PersonalizedJobPostingNotificationDto;

namespace Vettingo.NotificationService.UnitTests.Messaging;

public sealed class PersonalizedJobPostingNotificationTests
{
    [Fact]
    public async Task Evaluation_Message_Should_Match_Auth_Subscription_And_Contain_Only_UserId()
    {
        var cap = Substitute.For<ICapPublisher>();
        cap.PublishAsync(Arg.Any<string>(), Arg.Any<EvaluationDto>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var publisher = new EvaluationPublisher(cap);
        var dto = new EvaluationDto { UserId = Guid.NewGuid() };
        using var cancellation = new CancellationTokenSource();

        await publisher.PublishAsync(dto, cancellation.Token);

        var arguments = cap.ReceivedCalls().Single().GetArguments();
        var subscription = typeof(AuthConsumer)
            .GetMethod(nameof(AuthConsumer.HandleAsync))!
            .GetCustomAttribute<CapSubscribeAttribute>()!;
        arguments[0].Should().Be(EvaluationPublisher.TopicName);
        arguments[0].Should().Be(subscription.Name);
        subscription.Group.Should().Be("vettingo.auth-service");
        arguments[3].Should().Be(cancellation.Token);

        var json = JsonSerializer.Serialize(arguments[1], JsonSerializerOptions.Web);
        using var document = JsonDocument.Parse(json);
        document.RootElement.EnumerateObject().Select(property => property.Name)
            .Should().Equal("userId");
        JsonSerializer.Deserialize<AuthRequest>(json, JsonSerializerOptions.Web)!
            .UserId.Should().Be(dto.UserId);
    }

    [Fact]
    public async Task Auth_Message_Should_Match_Notification_Subscription_And_Include_Email()
    {
        var cap = Substitute.For<ICapPublisher>();
        cap.PublishAsync(Arg.Any<string>(), Arg.Any<PublishedDto>(),
            Arg.Any<string?>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        var publisher = new AuthPublisher(cap);
        var dto = new PublishedDto
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.NewGuid(),
            Email = "candidate@example.com"
        };
        using var cancellation = new CancellationTokenSource();

        await publisher.PublishAsync(dto, cancellation.Token);

        var arguments = cap.ReceivedCalls().Single().GetArguments();
        var subscription = typeof(PersonalizedJobPostingNotificationConsumer)
            .GetMethod(nameof(PersonalizedJobPostingNotificationConsumer.HandleAsync))!
            .GetCustomAttribute<CapSubscribeAttribute>()!;
        arguments[0].Should().Be(AuthPublisher.TopicName);
        arguments[0].Should().Be(subscription.Name);
        subscription.Group.Should().Be("vettingo.notification-service");
        arguments[3].Should().Be(cancellation.Token);

        var json = JsonSerializer.Serialize(arguments[1], JsonSerializerOptions.Web);
        using var document = JsonDocument.Parse(json);
        document.RootElement.EnumerateObject().Select(property => property.Name)
            .Should().BeEquivalentTo("id", "userId", "email");
        var received = JsonSerializer.Deserialize<ReceivedDto>(json, JsonSerializerOptions.Web)!;
        received.Id.Should().Be(dto.Id);
        received.UserId.Should().Be(dto.UserId);
        received.Email.Should().Be(dto.Email);
    }
}
