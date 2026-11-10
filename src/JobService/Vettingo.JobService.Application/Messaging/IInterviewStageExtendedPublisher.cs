using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IInterviewStageExtendedPublisher
{
    Task SendExtendedNotification(InterviewStageExtendedMessage message, CancellationToken cancellationToken = default);
}
