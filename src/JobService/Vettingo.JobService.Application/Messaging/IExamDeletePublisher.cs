using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IExamDeletePublisher
{
    Task PublishAsync(ExamDeleteMessage message, CancellationToken cancellationToken = default);
}
