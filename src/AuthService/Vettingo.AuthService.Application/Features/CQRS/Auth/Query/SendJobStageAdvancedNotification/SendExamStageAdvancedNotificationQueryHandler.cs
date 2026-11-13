using FlashMediator;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;

public sealed class SendExamStageAdvancedNotificationQueryHandler(
    JobStageNotificationRecipients recipients,
    IExamStageAdvancedNotificationPublisher publisher) : IRequestHandler<SendExamStageAdvancedNotificationQueryRequest>
{
    public async Task Handle(SendExamStageAdvancedNotificationQueryRequest request, CancellationToken cancellationToken)
    {
        var (companyName, candidates) = await recipients.ResolveAsync(
            request.CompanyId, request.NotificationIdsByCandidateId.Keys, cancellationToken);

        foreach (var candidate in candidates)
        {
            await publisher.PublishAsync(new ExamStageAdvancedNotificationMessage
            {
                Id = request.NotificationIdsByCandidateId[candidate.Id],
                CandidateId = candidate.Id,
                Email = candidate.Email,
                CompanyName = companyName,
                Title = request.Title,
                StartedDate = request.StartedDate,
                EndDate = request.EndDate
            }, cancellationToken);
        }
    }
}
