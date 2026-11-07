using DotNetCore.CAP;
using FlashMediator;
using Vettingo.JobService.Infrastructure.Messages;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class InterviewResultSubscriber : ICapSubscribe
{
    private readonly IMediator _mediator;

    public InterviewResultSubscriber(IMediator mediator)
    {
        _mediator = mediator;
    }

    [CapSubscribe("vettingo.job.interview-result.published.v1", Group = "vettingo.job-service")]
    public Task HandleAsync(InterviewResultMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
