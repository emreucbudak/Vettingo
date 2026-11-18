using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Messaging;

namespace Vettingo.JobService.Infrastructure.Messaging;

public sealed class ExamCreatePublisher(ICapPublisher capPublisher) : IExamCreatePublisher
{
    public Task PublishAsync(ExamCreateMessage message, CancellationToken cancellationToken = default)
        => capPublisher.PublishAsync(CandidateExamListPublisher.TopicName, message, cancellationToken: cancellationToken);
}
