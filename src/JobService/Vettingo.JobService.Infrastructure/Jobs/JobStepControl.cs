using Quartz;

namespace Vettingo.JobService.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public sealed class JobStepControl : IJob
{
    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;
}
