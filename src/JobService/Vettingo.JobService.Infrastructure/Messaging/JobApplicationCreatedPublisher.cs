using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class JobApplicationCreatedPublisher(ICapPublisher capPublisher) : IJobApplicationCreatedPublisher
{
    public Task PublishAsync(JobApplicationCreatedMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(
            "vettingo.auth.job-application-created.v1", message, cancellationToken: cancellationToken);
}
