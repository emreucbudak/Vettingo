using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class ResetPasswordCodeSenderPublisher(ICapPublisher capPublisher) : IResetPasswordCodeSenderPublisher
{
    public const string TopicName = "vettingo.notification.reset-password-code.requested.v1";

    public Task CodeSend(CodeMessage message, CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
