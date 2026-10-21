using DotNetCore.CAP;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class JobAnalyticsPublisher : IJobAnalyticsPublisher
{
    private readonly ICapPublisher _capPublisher;

    public JobAnalyticsPublisher(ICapPublisher capPublisher)
    {
        _capPublisher = capPublisher;
    }

    public Task PublishAsync(
        JobAnalyticsDto message,
        CancellationToken cancellationToken = default)
    {
        return _capPublisher.PublishAsync(
            JobAnalyticsDto.TopicName,
            message,
            cancellationToken: cancellationToken);
    }
}
