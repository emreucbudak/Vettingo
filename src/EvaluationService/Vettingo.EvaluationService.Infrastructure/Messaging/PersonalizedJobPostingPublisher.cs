using DotNetCore.CAP;
using Vettingo.EvaluationService.Application.DTOs;
using Vettingo.EvaluationService.Application.Messaging;

namespace Vettingo.EvaluationService.Infrastructure.Messaging;

public sealed class PersonalizedJobPostingPublisher(ICapPublisher capPublisher)
    : IPersonalizedJobPostingPublisher
{
    public Task PublishAsync(
        PersonalizedJobPostingDto posting,
        CancellationToken cancellationToken = default)
    {
        return capPublisher.PublishAsync(
            PersonalizedJobPostingDto.TopicName,
            posting,
            cancellationToken: cancellationToken);
    }
}
