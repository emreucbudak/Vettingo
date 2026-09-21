namespace Vettingo.ApplicationService.Application.Messaging;

public interface IJobApplicationCreatedPublisher
{
    Task PublishAsync(Guid jobPostingId, CancellationToken cancellationToken = default);
}
