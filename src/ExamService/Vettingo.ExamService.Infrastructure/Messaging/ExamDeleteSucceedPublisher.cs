using DotNetCore.CAP;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Application.Messaging;

namespace Vettingo.ExamService.Infrastructure.Messaging;

public sealed class ExamDeleteSucceedPublisher(ICapPublisher capPublisher) : IExamDeleteSucceedPublisher
{
    public Task PublishAsync(ExamDeleteSucceedMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync("vettingo.job.exam.delete.succeeded.v1", message, cancellationToken: cancellationToken);
}
