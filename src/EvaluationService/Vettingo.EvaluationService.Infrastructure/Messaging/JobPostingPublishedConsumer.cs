using DotNetCore.CAP;
using Vettingo.EvaluationService.Infrastructure.Messages;

namespace Vettingo.EvaluationService.Infrastructure.Messaging;

public sealed class JobPostingPublishedConsumer : ICapSubscribe
{
    [CapSubscribe("vettingo.evaluation.job-posting.published.v1", Group = "vettingo.evaluation-service")]
    public Task HandleAsync(
        JobPostingPublishedMessage message,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
