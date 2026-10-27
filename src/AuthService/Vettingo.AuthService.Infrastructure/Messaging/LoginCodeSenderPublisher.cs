using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class LoginCodeSenderPublisher(ICapPublisher capPublisher) : ILoginCodeSenderPublisher
{
    public const string TopicName = "vettingo.notification.login-code.requested.v1";

    public Task CodeSend(LoginCodeMessage message, CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
