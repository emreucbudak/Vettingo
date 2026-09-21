using DotNetCore.CAP;
using Vettingo.ApplicationService.Application.Messaging;

namespace Vettingo.ApplicationService.Infrastructure.Messaging;

public sealed class JobApplicationCreatedPublisher(ICapPublisher publisher) : IJobApplicationCreatedPublisher
{
    public Task PublishAsync(Guid jobPostingId, CancellationToken cancellationToken = default)
        => publisher.PublishAsync("vettingo.job-application.created.v1", jobPostingId, cancellationToken: cancellationToken);
}
