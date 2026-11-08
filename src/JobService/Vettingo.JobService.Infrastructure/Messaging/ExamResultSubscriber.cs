using DotNetCore.CAP;
using FlashMediator;
using Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.RejectFailedExamApplication;
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
    public async Task HandleAsync(ExamResultMessage message, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RejectFailedExamApplicationCommandRequest
        {
            UserId = message.UserId,
            JobId = message.JobId
        }, cancellationToken);
    }
}
