using DotNetCore.CAP;
using Vettingo.JobService.Application.DTOs;
using Vettingo.JobService.Application.Services;

namespace Vettingo.JobService.Infrastructure.Services
{
    public class NotificationSender : INotificationSender
    {
        private const string TopicName = "vettingo.job.personalized-job-posting.notification.requested.v1";
        private readonly ICapPublisher _capPublisher;

        public NotificationSender(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task SendAsync(PersonalizedJobPostingMessage message, CancellationToken cancellationToken = default)
        {
            await _capPublisher.PublishAsync(TopicName, message, cancellationToken: cancellationToken);
        }
    }
}
