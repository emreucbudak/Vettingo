using DotNetCore.CAP;
using Vettingo.NotificationService.Application.DTOs;

namespace Vettingo.NotificationService.Infrastructure.Messaging;

public sealed class RegisterConfirmCodeSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.notification.register-confirm-code.requested.v1")]
    public Task HandleAsync(CodeMessage message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
