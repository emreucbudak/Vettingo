using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class ExamStageAdvancedPublisher(ICapPublisher capPublisher) : IExamStageAdvancedPublisher
{
    public const string TopicName = "vettingo.auth.exam-stage-advanced.requested.v1";

    public Task PublishAsync(ExamStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
}
