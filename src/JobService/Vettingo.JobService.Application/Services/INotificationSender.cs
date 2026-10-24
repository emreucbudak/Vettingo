using Vettingo.JobService.Application.DTOs;

namespace Vettingo.JobService.Application.Services
{
    public interface INotificationSender
    {
        Task SendAsync(PersonalizedJobPostingMessage message, CancellationToken cancellationToken = default);
    }
}
