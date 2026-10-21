namespace Vettingo.JobService.Application.Messaging;

public interface IJobAnalyticsPublisher
{
    Task PublishAsync(
        JobAnalyticsDto message,
        CancellationToken cancellationToken = default);
}
