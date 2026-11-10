using DotNetCore.CAP;
using FlashMediator;
using Vettingo.InterviewService.Application.Features.CQRS.JobInterview.Command.Create;
using Vettingo.InterviewService.Infrastructure.Messages;

namespace Vettingo.InterviewService.Infrastructure.Messaging;

public sealed class InterviewCreateConsumer(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe("vettingo.interview.job-interview.create.requested.v1", Group = "vettingo.interview-service")]
    public async Task HandleAsync(InterviewCreateMessage message, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateJobInterviewCommandRequest
        {
            StepId = message.StepId,
            InterviewIntervalMinutes = message.InterviewIntervalMinutes
        }, cancellationToken);
    }
}
