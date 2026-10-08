using Vettingo.EvaluationService.Application.DTOs;

namespace Vettingo.EvaluationService.Application.Messaging;

public interface IPersonalizedJobPostingPublisher
{
    Task PublishAsync(
        PersonalizedJobPostingDto posting,
        CancellationToken cancellationToken = default);
}
