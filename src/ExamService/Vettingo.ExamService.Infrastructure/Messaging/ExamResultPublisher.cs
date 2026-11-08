using DotNetCore.CAP;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Messaging;

namespace Vettingo.ExamService.Infrastructure.Messaging;

public sealed class ExamResultPublisher : IExamResultPublisher
{
    public const string TopicName = "vettingo.job.exam-result.published.v1";

    private readonly ICapPublisher _capPublisher;

    public ExamResultPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public Task PublishAsync(ExamResultMessage message, CancellationToken cancellationToken = default)
    {
        return _capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
    }
}
