using DotNetCore.CAP;
using FlashMediator;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.MarkInterviewStepPassed;
using Vettingo.JobService.Infrastructure.Messages;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class InterviewResultSubscriber : ICapSubscribe
{
    private readonly IMediator _mediator;

    public InterviewResultSubscriber(IMediator mediator)
    {
        _mediator = mediator;
    }

    [CapSubscribe("vettingo.job.interview-result.passed.v1", Group = "vettingo.job-service")]
    public async Task HandleAsync(InterviewResultMessage message, CancellationToken cancellationToken)
    {
        await _mediator.Send(new MarkInterviewStepPassedCommandRequest
        {
            UserId = message.UserId,
            JobId = message.JobId
        }, cancellationToken);
    }
}
