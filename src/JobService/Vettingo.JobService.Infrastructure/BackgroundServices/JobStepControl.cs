using Microsoft.Extensions.Hosting;

namespace Vettingo.JobService.Infrastructure.BackgroundServices;

public sealed class JobStepControl : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromDays(1));

        do
        {
            await CheckDueStepsAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private static Task CheckDueStepsAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
