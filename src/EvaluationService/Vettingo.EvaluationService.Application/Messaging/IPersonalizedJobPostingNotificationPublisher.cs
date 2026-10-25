using Vettingo.EvaluationService.Application.DTOs;

namespace Vettingo.EvaluationService.Application.Messaging;

public interface IPersonalizedJobPostingNotificationPublisher
{
    Task PublishAsync(
        PersonalizedJobPostingNotificationDto notification,
        CancellationToken cancellationToken = default);
}
