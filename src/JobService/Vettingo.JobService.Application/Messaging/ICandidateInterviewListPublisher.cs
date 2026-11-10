using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface ICandidateInterviewListPublisher
{
    Task PublishAsync(InterviewCreateMessage message, CancellationToken cancellationToken = default);
}
