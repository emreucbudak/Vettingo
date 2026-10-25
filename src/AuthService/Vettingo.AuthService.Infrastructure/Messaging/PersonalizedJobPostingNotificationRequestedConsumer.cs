using DotNetCore.CAP;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Exceptions;
using Vettingo.AuthService.Application.Messaging;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingNotificationRequestedConsumer(
    UserManager<User> userManager,
    IPersonalizedJobPostingNotificationPublisher publisher,
    ILogger<PersonalizedJobPostingNotificationRequestedConsumer> logger) : ICapSubscribe
{
    [CapSubscribe(PersonalizedJobPostingNotificationRequestedMessage.TopicName, Group = "vettingo.auth-service")]
    public async Task HandleAsync(
        PersonalizedJobPostingNotificationRequestedMessage message,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (message.UserId == Guid.Empty)
            throw new BadRequestException("Kullanıcı kimliği boş olamaz.");

        var user = await userManager.FindByIdAsync(message.UserId.ToString("D"))
            ?? throw new NotFoundException("Bildirim gönderilecek kullanıcı bulunamadı.");
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new BusinessException("Bildirim gönderilecek kullanıcının e-posta adresi bulunamadı.");

        await publisher.PublishAsync(new PersonalizedJobPostingNotificationDto
        {
            UserId = user.Id,
            Email = user.Email
        }, cancellationToken);

        logger.LogInformation("Kişiselleştirilmiş ilan bildirim isteği Notification servisine iletildi. UserId: {UserId}", user.Id);
    }
}
