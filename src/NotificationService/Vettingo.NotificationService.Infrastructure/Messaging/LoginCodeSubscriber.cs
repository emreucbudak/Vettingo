using DotNetCore.CAP;
using Vettingo.NotificationService.Application.DTOs;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class LoginCodeSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.login-code.requested.v1")]
    public Task HandleAsync(CodeMessage message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
