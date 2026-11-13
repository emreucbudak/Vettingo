using DotNetCore.CAP;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Infrastructure.Messaging;

public sealed class JobApplicationCreatedNotificationPublisher(ICapPublisher capPublisher)
    : IJobApplicationCreatedNotificationPublisher
{
    public Task PublishAsync(JobApplicationCreatedNotificationMessage message, CancellationToken cancellationToken)
        => capPublisher.PublishAsync(
            "vettingo.notification.job-application-created.v1", message, cancellationToken: cancellationToken);
}
