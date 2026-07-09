using FluentValidation;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandRequestValidator : AbstractValidator<MarkNotificationAsReadCommandRequest>
{
    public MarkNotificationAsReadCommandRequestValidator()
    {
        RuleFor(x => x.NotificationId).NotEmpty();
    }
}

