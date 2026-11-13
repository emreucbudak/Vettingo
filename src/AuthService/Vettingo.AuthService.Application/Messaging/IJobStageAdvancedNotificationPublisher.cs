using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IJobStageAdvancedNotificationPublisher
{
    Task PublishAsync(JobStageAdvancedNotificationMessage message, CancellationToken cancellationToken = default);
}
