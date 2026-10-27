using DotNetCore.CAP;
using Vettingo.NotificationService.Application.DTOs;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class ResetPasswordCodeSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.reset-password-code.requested.v1")]
    public Task HandleAsync(CodeMessage message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
