using FlashMediator;
using NSubstitute;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messages;
using Vettingo.NotificationService.Infrastructure.Messaging;

namespace Vettingo.NotificationService.UnitTests.Messaging;

public sealed class JobApplicationCreatedSubscriberTests
{
    [Fact]
    public async Task HandleAsync_Should_Create_Application_Confirmation()
    {
        var mediator = Substitute.For<IMediator>();
        var emailSender = Substitute.For<IEmailSender>();
        var message = new JobApplicationCreatedMessage
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.NewGuid(),
            Email = "candidate@example.com"
        };
        var subscriber = new JobApplicationCreatedSubscriber(mediator, emailSender);

        await subscriber.HandleAsync(message, CancellationToken.None);

        await mediator.Received(1).Send(Arg.Is<CreateNotificationCommandRequest>(request =>
            request.Id == message.Id && request.UserId == message.UserId && request.IgnoreDuplicateId &&
            request.Title == "Başvurun kaydedildi" && request.Message.Contains("Başvurularım")),
            CancellationToken.None);
        await emailSender.Received(1).Send(Arg.Is<EmailMessage>(email =>
            email.To == message.Email && email.Body.Contains("Başvurun kaydedildi") &&
            email.Body.Contains("Başvurularım")));
    }
}
