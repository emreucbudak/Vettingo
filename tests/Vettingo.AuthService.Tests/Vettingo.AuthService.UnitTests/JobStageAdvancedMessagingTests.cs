using DotNetCore.CAP;
using FlashMediator;
using FluentAssertions;
using NSubstitute;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;
using Vettingo.AuthService.Infrastructure.Messages;
using Vettingo.AuthService.Infrastructure.Messaging;

namespace Vettingo.AuthService.UnitTests;

public sealed class JobStageAdvancedMessagingTests
{
    [Fact]
    public async Task Offer_Notification_Uses_Its_Own_Cap_Topic()
    {
        var capPublisher = Substitute.For<ICapPublisher>();
        var message = new OfferStageAdvancedNotificationMessage
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Email = "candidate@example.com",
            CompanyName = "Vettingo",
            Title = "Backend Developer",
            StartedDate = new DateOnly(2026, 11, 16),
            EndDate = new DateOnly(2026, 11, 20)
        };

        await new OfferStageAdvancedNotificationPublisher(capPublisher).PublishAsync(message);

        var arguments = capPublisher.ReceivedCalls().Should().ContainSingle().Subject.GetArguments();
        arguments[0].Should().Be("vettingo.notification.offer-stage-advanced.requested.v1");
        arguments[1].Should().BeSameAs(message);
    }

    [Fact]
    public async Task InterviewSubscriber_Forwards_Notification_Id_And_Chapter()
    {
        var mediator = Substitute.For<IMediator>();
        var candidateId = Guid.NewGuid();
        var notificationId = Guid.CreateVersion7();
        var message = new InterviewStageAdvancedRequestedMessage
        {
            NotificationIdsByCandidateId = new Dictionary<Guid, Guid> { [candidateId] = notificationId },
            CompanyId = Guid.NewGuid(),
            Title = "Backend Developer",
            StartedDate = new DateOnly(2026, 11, 16),
            EndDate = new DateOnly(2026, 11, 20),
            InterviewChapterName = "Teknik Mülakat"
        };

        await new InterviewStageAdvancedSubscriber(mediator).HandleAsync(message, CancellationToken.None);

        await mediator.Received(1).Send(
            Arg.Is<SendInterviewStageAdvancedNotificationQueryRequest>(request =>
                request.NotificationIdsByCandidateId[candidateId] == notificationId &&
                request.InterviewChapterName == message.InterviewChapterName &&
                request.CompanyId == message.CompanyId &&
                request.Title == message.Title &&
                request.StartedDate == message.StartedDate &&
                request.EndDate == message.EndDate), CancellationToken.None);
    }

    [Fact]
    public async Task Subscriber_Forwards_Candidate_Ids_And_Stage_Details_To_Query()
    {
        var mediator = Substitute.For<IMediator>();
        var subscriber = new JobStageAdvancedSubscriber(mediator);
        var message = new JobStageAdvancedRequestedMessage
        {
            CandidateIds = [Guid.NewGuid(), Guid.NewGuid()],
            CompanyId = Guid.NewGuid(),
            Title = "Backend Developer",
            StartedDate = new DateOnly(2026, 11, 16),
            EndDate = new DateOnly(2026, 11, 20),
            StageType = "Interview"
        };
        using var cancellation = new CancellationTokenSource();

        await subscriber.HandleAsync(message, cancellation.Token);

        await mediator.Received(1).Send(
            Arg.Is<SendJobStageAdvancedNotificationQueryRequest>(request =>
                request.CandidateIds.SequenceEqual(message.CandidateIds) &&
                request.CompanyId == message.CompanyId &&
                request.Title == message.Title &&
                request.StartedDate == message.StartedDate &&
                request.EndDate == message.EndDate &&
                request.StageType == message.StageType),
            cancellation.Token);
    }

    [Fact]
    public async Task Publisher_Forwards_Resolved_Notification_To_CAP()
    {
        var capPublisher = Substitute.For<ICapPublisher>();
        var publisher = new JobStageAdvancedNotificationPublisher(capPublisher);
        var message = new JobStageAdvancedNotificationMessage
        {
            CandidateId = Guid.NewGuid(),
            Email = "candidate@example.com",
            CompanyName = "Vettingo",
            Title = "Backend Developer",
            StartedDate = new DateOnly(2026, 11, 16),
            EndDate = new DateOnly(2026, 11, 20),
            StageType = "Exam"
        };
        using var cancellation = new CancellationTokenSource();

        await publisher.PublishAsync(message, cancellation.Token);

        var arguments = capPublisher.ReceivedCalls().Should().ContainSingle().Subject.GetArguments();
        arguments[0].Should().Be("vettingo.notification.job-stage-advanced.requested.v1");
        arguments[1].Should().BeSameAs(message);
        arguments[3].Should().Be(cancellation.Token);
    }
}
