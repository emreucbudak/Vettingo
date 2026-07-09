using FluentValidation;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;

public sealed class CreateNotificationCommandRequestValidator : AbstractValidator<CreateNotificationCommandRequest>
{
    public CreateNotificationCommandRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Type).IsInEnum();
    }
}

