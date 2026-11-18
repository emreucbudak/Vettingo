using Vettingo.ExamService.Application.DTOs;

namespace Vettingo.ExamService.Application.Messaging;

public interface IExamDeleteSucceedPublisher
{
    Task PublishAsync(ExamDeleteSucceedMessage message, CancellationToken cancellationToken = default);
}
