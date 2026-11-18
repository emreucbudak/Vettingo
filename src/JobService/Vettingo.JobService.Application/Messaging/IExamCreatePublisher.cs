using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Messaging;

public interface IExamCreatePublisher
{
    Task PublishAsync(ExamCreateMessage message, CancellationToken cancellationToken = default);
}
