using DotNetCore.CAP;
using FlashMediator;
using Vettingo.JobService.Infrastructure.Messages;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class ExamResultSubscriber : ICapSubscribe
{
    private readonly IMediator _mediator;

    public ExamResultSubscriber(IMediator mediator)
    {
        _mediator = mediator;
    }

    [CapSubscribe("vettingo.job.exam-result.published.v1", Group = "vettingo.job-service")]
    public Task HandleAsync(ExamResultMessage message, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
