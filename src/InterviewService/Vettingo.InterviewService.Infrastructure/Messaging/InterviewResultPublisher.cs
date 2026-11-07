using DotNetCore.CAP;
using Vettingo.InterviewService.Application.DTOs;
using Vettingo.InterviewService.Application.Messaging;

namespace Vettingo.InterviewService.Infrastructure.Messaging;

public sealed class InterviewResultPublisher : IInterviewResultPublisher
{
    public const string TopicName = "vettingo.job.interview-result.published.v1";

    private readonly ICapPublisher _capPublisher;

    public InterviewResultPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public Task PublishAsync(InterviewResultMessage message, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
