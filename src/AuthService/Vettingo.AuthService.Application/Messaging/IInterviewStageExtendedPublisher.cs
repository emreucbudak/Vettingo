using Vettingo.AuthService.Application.DTOs;

namespace Vettingo.AuthService.Application.Messaging;

public interface IInterviewStageExtendedPublisher
{
    Task SendExtendedNotification(InterviewStageExtendedMessage message, CancellationToken cancellationToken = default);
}
