using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IExamStageAdvancedPublisher
{
    Task PublishAsync(ExamStageAdvancedRequestedMessage message, CancellationToken cancellationToken = default);
}
