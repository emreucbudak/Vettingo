using FlashMediator;
using Quartz;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.AdvanceJobStep;

namespace Vettingo.JobService.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public sealed class JobStepControl(IMediator mediator) : IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new AdvanceJobStepCommandRequest(), cancellationToken);
    }
}
