using Vettingo.ExamService.Application.DTOs;

namespace Vettingo.ExamService.Application.Messaging;

public interface IExamResultPublisher
{
    Task PublishAsync(ExamResultMessage message, CancellationToken cancellationToken = default);
}
