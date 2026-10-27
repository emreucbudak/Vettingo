using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class RegisterConfirmCodeSenderPublisher(ICapPublisher capPublisher) : IRegisterConfirmCodeSenderPublisher
{
    public const string TopicName = "vettingo.notification.register-confirm-code.requested.v1";

    public Task CodeSend(CodeMessage message, CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
