using System.Net;
using FlashMediator;
using NSubstitute;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messaging;
using Vettingo.NotificationService.Infrastructure.Messages;

namespace Vettingo.NotificationService.UnitTests.Messaging;

public sealed class JobStageAdvancedSubscriberTests
{
    [Fact]
    public async Task Offer_Subscriber_Saves_Notification_And_Sends_Offer_Window()
    {
        var mediator = Substitute.For<IMediator>();
        var emailSender = Substitute.For<IEmailSender>();
        var message = new OfferStageAdvancedNotificationMessage
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            Email = "candidate@example.com",
            CompanyName = "Vettingo & Partners",
            Title = "Backend <Developer>",
            StartedDate = new DateOnly(2026, 11, 13),
            EndDate = new DateOnly(2026, 11, 17)
        };

        await new OfferStageAdvancedSubscriber(mediator, emailSender).HandleAsync(message, CancellationToken.None);

        Received.InOrder(() =>
        {
            _ = mediator.Send(Arg.Is<CreateNotificationCommandRequest>(request =>
                request.Id == message.Id && request.UserId == message.UserId && request.IgnoreDuplicateId &&
                request.Message.Contains("son aşama olan teklif") &&
                request.Message.Contains("13 Kasım 2026 ile 17 Kasım 2026")), CancellationToken.None);
            _ = emailSender.Send(Arg.Is<EmailMessage>(email =>
                email.To == message.Email && email.IsBodyHtml &&
                email.Body.Contains("Vettingo &amp; Partners") && email.Body.Contains("Backend &lt;Developer&gt;") &&
                email.Body.Contains("13 Kasım 2026 ile 17 Kasım 2026") &&
                email.Body.Contains("bildirim kutunuzu ve e-postanızı")));
        });
    }

    [Theory]
    [InlineData(false, "sınav")]
    [InlineData(true, "mülakat")]
    public async Task Separate_Subscribers_Save_The_Incoming_Id_And_Send_Stage_Email(bool interview, string stage)
    {
        var mediator = Substitute.For<IMediator>();
        var emailSender = Substitute.For<IEmailSender>();
        var delivery = new JobStageNotificationDelivery(mediator, emailSender);
        var id = Guid.CreateVersion7();
        var candidateId = Guid.NewGuid();
        var startedDate = new DateOnly(2026, 11, 13);
        var endDate = new DateOnly(2026, 11, 17);
        const string email = "candidate@example.com";
        const string chapter = "Teknik <Mülakat>";

        if (interview)
        {
            await new InterviewStageAdvancedSubscriber(delivery).HandleAsync(new InterviewStageAdvancedNotificationMessage
            {
                Id = id, CandidateId = candidateId, Email = email, CompanyName = "Vettingo", Title = "Backend",
                StartedDate = startedDate, EndDate = endDate, InterviewChapterName = chapter
            }, CancellationToken.None);
        }
        else
        {
            await new ExamStageAdvancedSubscriber(delivery).HandleAsync(new ExamStageAdvancedNotificationMessage
            {
                Id = id, CandidateId = candidateId, Email = email, CompanyName = "Vettingo", Title = "Backend",
                StartedDate = startedDate, EndDate = endDate
            }, CancellationToken.None);
        }

        Received.InOrder(() =>
        {
            _ = mediator.Send(Arg.Is<CreateNotificationCommandRequest>(request =>
                request.Id == id && request.UserId == candidateId && request.IgnoreDuplicateId &&
                request.Title.Contains(stage) && request.Message.Contains("17 Kasım 2026") &&
                request.Message.Contains(chapter) == interview), CancellationToken.None);
            _ = emailSender.Send(Arg.Is<EmailMessage>(message =>
                message.To == email && message.IsBodyHtml && message.Body.Contains($"Yeni bir {stage} aşamasına geçtin!") &&
                message.Body.Contains("17 Kasım 2026") && WebUtility.HtmlDecode(message.Body).Contains("Başvurularım") == interview &&
                message.Body.Contains(WebUtility.HtmlEncode(chapter)) == interview));
        });
    }

    [Theory]
    [InlineData("Exam", "sınav")]
    [InlineData("Interview", "mülakat")]
    public async Task HandleAsync_Should_Send_Html_Email_For_The_Stage(string stageType, string stageName)
    {
        var emailSender = Substitute.For<IEmailSender>();
        var subscriber = new JobStageAdvancedSubscriber(emailSender);
        var message = new JobStageAdvancedMessage
        {
            CandidateId = Guid.NewGuid(),
            Email = "candidate@example.com",
            CompanyName = "Vettingo & Partners",
            Title = "Backend <Developer>",
            StartedDate = new DateOnly(2026, 11, 13),
            EndDate = new DateOnly(2026, 11, 17),
            StageType = stageType
        };

        await subscriber.HandleAsync(message, CancellationToken.None);

        await emailSender.Received(1).Send(Arg.Is<EmailMessage>(email =>
            email.To == message.Email && email.IsBodyHtml &&
            email.Body.Contains($"Yeni bir {stageName} aşamasına geçtin!") &&
            email.Body.Contains("Vettingo &amp; Partners") && email.Body.Contains("Backend &lt;Developer&gt;") &&
            email.Body.Contains("13 Kasım 2026") && email.Body.Contains("17 Kasım 2026") &&
            email.Body.Contains("bu aşamada başarısız sayılırsın")));
    }
}
