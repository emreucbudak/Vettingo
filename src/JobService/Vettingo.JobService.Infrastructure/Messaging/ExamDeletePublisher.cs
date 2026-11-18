using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class ExamDeletePublisher(ICapPublisher capPublisher) : IExamDeletePublisher
{
    public Task PublishAsync(ExamDeleteMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync("vettingo.exam.exam.delete.requested.v1", message, cancellationToken: cancellationToken);
}
