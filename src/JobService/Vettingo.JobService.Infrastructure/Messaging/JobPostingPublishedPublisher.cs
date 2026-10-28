using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class JobPostingPublishedPublisher(ICapPublisher capPublisher)
    : IJobPostingPublishedPublisher
{
    public const string TopicName = "vettingo.evaluation.job-posting.published.v1";

    public Task PublishAsync(
        JobPostingPublishedMessage message,
        CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(
            TopicName,
            message,
            cancellationToken: cancellationToken);
    }
}
