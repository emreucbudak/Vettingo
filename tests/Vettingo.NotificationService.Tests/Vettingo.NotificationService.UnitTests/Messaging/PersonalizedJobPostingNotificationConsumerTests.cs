using FlashMediator;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Repository;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Domain.Entities;
using Vettingo.NotificationService.Infrastructure.Messaging;
using Vettingo.NotificationService.Persistence.DbContext;
using Vettingo.NotificationService.Persistence.Repository;

namespace Vettingo.NotificationService.UnitTests.Messaging;

public sealed class PersonalizedJobPostingNotificationConsumerTests
{
    [Fact]
    public async Task Message_Should_Create_And_Deliver_Notification_Then_Email_Received_Address()
    {
        var emailSender = Substitute.For<IEmailSender>();
        var notificationSender = Substitute.For<INotificationSender>();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<NotificationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddSingleton(notificationSender);
        services.AddFlashMediator(typeof(CreateNotificationCommandHandler).Assembly);
        services.AddFlashMediatorHybridCache();
        await using var provider = services.BuildServiceProvider();
        await using var scope = provider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var consumer = CreateConsumer(emailSender, scope.ServiceProvider.GetRequiredService<IMediator>());
        var message = Message();
        using var cancellation = new CancellationTokenSource();
        emailSender.When(sender => sender.Send(Arg.Any<EmailMessage>())).Do(_ =>
            context.Notifications.Local.Should().ContainSingle());

        await consumer.HandleAsync(message, cancellation.Token);

        context.ChangeTracker.Clear();
        var notification = await context.Notifications.SingleAsync();
        notification.UserId.Should().Be(message.UserId);
        notification.Title.Should().Be("Yeni bir kişiselleştirilmiş ilanın var!");
        notification.Message.Should().Contain("profilindeki Kişiselleştirilmiş İlanlar sayfasına");
        notification.IsRead.Should().BeFalse();
        await notificationSender.Received(1).SendNotificationAsync(
            Arg.Is<Notification>(sent => sent.Id == notification.Id), cancellation.Token);
        var email = (EmailMessage)emailSender.ReceivedCalls().Single().GetArguments()[0]!;
        email.To.Should().Be(message.Email);
        email.Body.Should().Contain("yeni bir kişiselleştirilmiş iş ilanı var!")
            .And.Contain("Hemen Vettingo'ya gelerek")
            .And.Contain("profilindeki Kişiselleştirilmiş İlanlar sayfasını");
    }

    [Fact]
    public async Task Notification_Failure_Should_Propagate_And_Not_Send_Email()
    {
        var emailSender = Substitute.For<IEmailSender>();
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<CreateNotificationCommandRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<CreateNotificationCommandResponse>(new InvalidOperationException("Database unavailable")));
        var consumer = CreateConsumer(emailSender, mediator);

        Func<Task> handle = () => consumer.HandleAsync(Message(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database unavailable");
        emailSender.ReceivedCalls().Should().BeEmpty();
    }

    [Fact]
    public async Task Handler_Should_Wait_For_Email_Delivery_To_Complete()
    {
        var emailSender = Substitute.For<IEmailSender>();
        var emailCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        emailSender.Send(Arg.Any<EmailMessage>()).Returns(emailCompletion.Task);
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<CreateNotificationCommandRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new CreateNotificationCommandResponse()));
        var consumer = CreateConsumer(emailSender, mediator);

        var handling = consumer.HandleAsync(Message(), CancellationToken.None);

        handling.IsCompleted.Should().BeFalse();
        emailCompletion.SetResult();
        await handling;
    }

    [Fact]
    public async Task Email_Failure_Should_Propagate_To_CAP_For_Retry()
    {
        var emailSender = Substitute.For<IEmailSender>();
        emailSender.Send(Arg.Any<EmailMessage>())
            .Returns(Task.FromException(new InvalidOperationException("SMTP unavailable")));
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<CreateNotificationCommandRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new CreateNotificationCommandResponse()));
        var consumer = CreateConsumer(emailSender, mediator);

        Func<Task> handle = () => consumer.HandleAsync(Message(), CancellationToken.None);

        await handle.Should().ThrowAsync<InvalidOperationException>().WithMessage("SMTP unavailable");
    }

    private static PersonalizedJobPostingNotificationDto Message() => new()
    {
        UserId = Guid.NewGuid(),
        Email = "candidate@example.com"
    };

    private static PersonalizedJobPostingNotificationConsumer CreateConsumer(IEmailSender emailSender, IMediator mediator) =>
        new(NullLogger<PersonalizedJobPostingNotificationConsumer>.Instance, emailSender, mediator);
}
