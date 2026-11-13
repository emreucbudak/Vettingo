using FlashMediator;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;

public sealed class SendJobStageAdvancedNotificationQueryHandler(
    JobStageNotificationRecipients recipients,
    IJobStageAdvancedNotificationPublisher publisher)
    : IRequestHandler<SendJobStageAdvancedNotificationQueryRequest>
{
    public async Task Handle(SendJobStageAdvancedNotificationQueryRequest request, CancellationToken cancellationToken)
    {
        var (companyName, candidates) = await recipients.ResolveAsync(
            request.CompanyId, request.CandidateIds.Distinct(), cancellationToken);

        foreach (var candidate in candidates)
        {
            await publisher.PublishAsync(new JobStageAdvancedNotificationMessage
            {
                CandidateId = candidate.Id,
                Email = candidate.Email,
                CompanyName = companyName,
                Title = request.Title,
                StartedDate = request.StartedDate,
                EndDate = request.EndDate,
                StageType = request.StageType
            }, cancellationToken);
        }
    }
}
