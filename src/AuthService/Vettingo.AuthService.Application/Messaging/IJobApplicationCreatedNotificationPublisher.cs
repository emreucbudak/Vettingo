using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IJobApplicationCreatedNotificationPublisher
{
    Task PublishAsync(JobApplicationCreatedNotificationMessage message, CancellationToken cancellationToken);
}
