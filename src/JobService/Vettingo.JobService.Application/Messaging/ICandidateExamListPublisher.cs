using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface ICandidateExamListPublisher
{
    Task PublishAsync(ContinuingCandidateListMessage message, CancellationToken cancellationToken = default);
}
