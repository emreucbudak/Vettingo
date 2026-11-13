using FlashMediator;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;

public sealed class SendInterviewStageAdvancedNotificationQueryHandler(
    JobStageNotificationRecipients recipients,
    IInterviewStageAdvancedNotificationPublisher publisher) : IRequestHandler<SendInterviewStageAdvancedNotificationQueryRequest>
{
    public async Task Handle(SendInterviewStageAdvancedNotificationQueryRequest request, CancellationToken cancellationToken)
    {
        var (companyName, candidates) = await recipients.ResolveAsync(
            request.CompanyId, request.NotificationIdsByCandidateId.Keys, cancellationToken);

        foreach (var candidate in candidates)
        {
            await publisher.PublishAsync(new InterviewStageAdvancedNotificationMessage
            {
                Id = request.NotificationIdsByCandidateId[candidate.Id],
                CandidateId = candidate.Id,
                Email = candidate.Email,
                CompanyName = companyName,
                Title = request.Title,
                StartedDate = request.StartedDate,
                EndDate = request.EndDate,
                InterviewChapterName = request.InterviewChapterName
            }, cancellationToken);
        }
    }
}
