using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class CandidateExamListPublisher : ICandidateExamListPublisher
{
    public const string TopicName = "vettingo.exam.job-stage-exam.create.requested.v1";

    private readonly ICapPublisher _capPublisher;

    public CandidateExamListPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public Task PublishAsync(ContinuingCandidateListMessage message, CancellationToken cancellationToken = default)
    {
        return _capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
