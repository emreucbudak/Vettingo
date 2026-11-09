using DotNetCore.CAP;
using FlashMediator;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.MarkExamStepPassed;
using Vettingo.JobService.Infrastructure.Messages;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class ExamResultSubscriber(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe("vettingo.job.exam-result.passed.v1", Group = "vettingo.job-service")]
    public async Task HandleAsync(ExamResultMessage message, CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkExamStepPassedCommandRequest
        {
            UserId = message.UserId,
            JobId = message.JobId
        }, cancellationToken);
    }
}
