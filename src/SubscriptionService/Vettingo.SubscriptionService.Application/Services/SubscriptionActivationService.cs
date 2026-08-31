using FlashMediator;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;
using Vettingo.SubscriptionService.Application.Features.CQRS.CompanySubscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Application.Messaging;

namespace Vettingo.SubscriptionService.Application.Services;

public sealed class SubscriptionActivationService(
    IMediator mediator,
    ISubscriptionRegistrationPublisher registrationPublisher)
    : ISubscriptionActivationService
{
    public async Task ActivateAsync(
        string accountType,
        Guid subscriberId,
        int planId,
        string billingPeriod,
        Guid registrationToken,
        CancellationToken cancellationToken = default)
    {
        DateTime startDateUtc = DateTime.UtcNow;
        DateTime endDateUtc = string.Equals(
            billingPeriod,
            "annual",
            StringComparison.Ordinal)
            ? startDateUtc.AddYears(1)
            : startDateUtc.AddMonths(1);

        switch (accountType)
        {
            case "candidate":
                await mediator.Send(
                    new CreateCandidateSubscriptionCommandRequest
                    {
                        CandidateId = subscriberId,
                        PlanId = planId,
                        StartDate = startDateUtc,
                        EndDate = endDateUtc
                    },
                    cancellationToken);
                break;

            case "employer":
                await mediator.Send(
                    new CreateCompanySubscriptionCommandRequest
                    {
                        CompanyId = subscriberId,
                        PlanId = planId,
                        StartDate = startDateUtc,
                        EndDate = endDateUtc
                    },
                    cancellationToken);
                break;

            default:
                throw new BadRequestException("Geçersiz hesap türü.");
        }

        await registrationPublisher.PublishRegistrationRequestedAsync(
            new SubscriptionRegistrationRequestedEvent
            {
                AccountType = accountType,
                RegistrationToken = registrationToken,
                SubscriberId = subscriberId
            },
            cancellationToken);
    }
}
