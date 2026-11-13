using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IJobApplicationCreatedPublisher
{
    Task PublishAsync(JobApplicationCreatedMessage message, CancellationToken cancellationToken = default);
}
