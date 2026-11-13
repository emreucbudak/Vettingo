using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class JobStageAdvancedPublisher(ICapPublisher capPublisher) : IJobStageAdvancedPublisher
{
    public const string TopicName = "vettingo.auth.job-stage-advanced.requested.v1";

    public Task PublishAsync(JobStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
