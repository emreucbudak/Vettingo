using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IExamStageAdvancedNotificationPublisher
{
    Task PublishAsync(ExamStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default);
}
