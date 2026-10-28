using DotNetCore.CAP;

namespace Vettingo.EvaluationService.Infrastructure.Messaging;

public sealed class JobPostingPublishedSubscriber : ICapSubscribe
{
    [CapSubscribe("vettingo.evaluation.job-posting.published.v1", Group = "vettingo.evaluation-service")]
    public Task HandleAsync(
        JobPostingPublishedMessage message,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
