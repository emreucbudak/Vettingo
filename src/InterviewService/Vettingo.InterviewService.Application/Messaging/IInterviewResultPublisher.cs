using Vettingo.InterviewService.Application.DTOs;

namespace Vettingo.InterviewService.Application.Messaging;

public interface IInterviewResultPublisher
{
    Task PublishAsync(InterviewResultMessage message, CancellationToken cancellationToken = default);
}
