using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IJobPostingPublishedPublisher
{
    Task PublishAsync(
        JobPostingPublishedMessage message,
        CancellationToken cancellationToken = default);
}
