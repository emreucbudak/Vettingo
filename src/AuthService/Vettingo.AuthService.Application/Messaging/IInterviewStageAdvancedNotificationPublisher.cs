using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IInterviewStageAdvancedNotificationPublisher
{
    Task PublishAsync(InterviewStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default);
}
