using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IJobStageAdvancedPublisher
{
    Task PublishAsync(JobStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default);
}
