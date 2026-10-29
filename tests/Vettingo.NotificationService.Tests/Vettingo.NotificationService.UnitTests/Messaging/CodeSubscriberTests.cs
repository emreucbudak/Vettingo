using FluentAssertions;
using NSubstitute;
using Vettingo.NotificationService.Application.DTOs;
using Vettingo.NotificationService.Application.Services;
using Vettingo.NotificationService.Infrastructure.Messages;
using Vettingo.NotificationService.Infrastructure.Messaging;

namespace Vettingo.NotificationService.UnitTests.Messaging;

public sealed class CodeSubscriberTests
{
    [Theory]
    [InlineData("login", "GİRİŞ KODUN")]
    [InlineData("register", "E-POSTA ONAY KODUN")]
    [InlineData("reset", "ŞİFRE SIFIRLAMA KODUN")]
    public async Task Message_Should_Send_Its_Html_Template_And_Code_To_Received_Email_And_Await_Delivery(
        string subscriber, string expectedHeading)
    {
        var emailSender = Substitute.For<IEmailSender>();
        var delivery = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        emailSender.Send(Arg.Any<EmailMessage>()).Returns(delivery.Task);
        var handle = CreateHandler(subscriber, emailSender);
        var message = new CodeMessage { Email = "candidate@example.com", Code = "047219" };

        var handling = handle(message, CancellationToken.None);

        var email = (EmailMessage)emailSender.ReceivedCalls().Single().GetArguments()[0]!;
        email.To.Should().Be(message.Email);
        email.IsBodyHtml.Should().BeTrue();
        email.Body.Should().Contain(expectedHeading)
            .And.Contain($">{message.Code}</td>")
            .And.Contain("Vettingo")
            .And.NotContain("{{Code}}");
        handling.IsCompleted.Should().BeFalse();

        delivery.SetResult();
        await handling;
    }

    [Theory]
    [InlineData("login")]
    [InlineData("register")]
    [InlineData("reset")]
    public async Task Email_Failure_Should_Propagate_To_CAP_For_Retry(string subscriber)
    {
        var emailSender = Substitute.For<IEmailSender>();
        emailSender.Send(Arg.Any<EmailMessage>())
            .Returns(Task.FromException(new InvalidOperationException("SMTP unavailable")));
        var handle = CreateHandler(subscriber, emailSender);
        var message = new CodeMessage { Email = "candidate@example.com", Code = "047219" };

        Func<Task> handling = () => handle(message, CancellationToken.None);

        await handling.Should().ThrowAsync<InvalidOperationException>().WithMessage("SMTP unavailable");
    }

    private static Func<CodeMessage, CancellationToken, Task> CreateHandler(string subscriber, IEmailSender emailSender) =>
        subscriber switch
        {
            "login" => new LoginCodeSubscriber(emailSender).HandleAsync,
            "register" => new RegisterConfirmCodeSubscriber(emailSender).HandleAsync,
            "reset" => new ResetPasswordCodeSubscriber(emailSender).HandleAsync,
            _ => throw new ArgumentOutOfRangeException(nameof(subscriber))
        };
}
