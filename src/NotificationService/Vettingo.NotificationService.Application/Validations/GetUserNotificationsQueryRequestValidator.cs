using FluentValidation;

namespace Vettingo.NotificationService.Application.Features.CQRS.Notification.Query.GetByUser;

public sealed class GetUserNotificationsQueryRequestValidator : AbstractValidator<GetUserNotificationsQueryRequest>
{
    public GetUserNotificationsQueryRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

