using DotNetCore.CAP;
using Microsoft.AspNetCore.Identity;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Infrastructure.Messages;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class JobApplicationCreatedSubscriber(
    UserManager<User> userManager,
    IJobApplicationCreatedNotificationPublisher publisher) : ICapSubscribe
{
    [CapSubscribe("vettingo.auth.job-application-created.v1", Group = "vettingo.auth-service")]
    public async Task HandleAsync(JobApplicationCreatedMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(message.UserId.ToString("D"))
            ?? throw new NotFoundException("Bildirim gönderilecek kullanıcı bulunamadı.");
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new BusinessException("Bildirim gönderilecek kullanıcının e-posta adresi bulunamadı.");
        }

        await publisher.PublishAsync(new JobApplicationCreatedNotificationMessage
        {
            Id = message.Id,
            UserId = message.UserId,
            Email = user.Email
        }, cancellationToken);
    }
}
