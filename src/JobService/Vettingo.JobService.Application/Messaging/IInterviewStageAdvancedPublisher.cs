using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IInterviewStageAdvancedPublisher
{
    Task PublishAsync(InterviewStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default);
}
