using FlashMediator;
using Vettingo.AuthService.Application.DTOs;
using Vettingo.AuthService.Application.Messaging;

namespace Vettingo.AuthService.Application.Features.CQRS.Auth.Query.SendJobStageAdvancedNotification;

public sealed class SendOfferStageAdvancedNotificationQueryHandler(
    JobStageNotificationRecipients recipients,
    IOfferStageAdvancedNotificationPublisher publisher) : IRequestHandler<SendOfferStageAdvancedNotificationQueryRequest>
{
    public async Task Handle(SendOfferStageAdvancedNotificationQueryRequest request, CancellationToken cancellationToken)
    {
        var (companyName, candidates) = await recipients.ResolveAsync(
            request.CompanyId, request.NotificationIdsByCandidateId.Keys, cancellationToken);

        foreach (var candidate in candidates)
        {
            await publisher.PublishAsync(new OfferStageAdvancedNotificationMessage
            {
                Id = request.NotificationIdsByCandidateId[candidate.Id],
                UserId = candidate.Id,
                CompanyId = request.CompanyId,
                Email = candidate.Email,
                CompanyName = companyName,
                Title = request.Title,
                StartedDate = request.StartedDate,
                EndDate = request.EndDate
            }, cancellationToken);
        }
    }
}
