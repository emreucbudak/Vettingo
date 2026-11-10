using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class CandidateInterviewListPublisher : ICandidateInterviewListPublisher
{
    public const string TopicName = "vettingo.interview.job-interview.create.requested.v1";

    private readonly ICapPublisher _capPublisher;

    public CandidateInterviewListPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public Task PublishAsync(InterviewCreateMessage message, CancellationToken cancellationToken = default)
    {
        return _capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
