using DotNetCore.CAP;
using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Infrastructure.Messaging;
using Vettingo.SubscriptionService.Infrastructure.Options;

namespace Vettingo.SubscriptionService.Infrastructure.Subscribers;

public sealed class CompanyCreatedCapSubscriber(
    IMediator mediator,
    CompanySubscriptionOptions subscriptionOptions,
    TimeProvider timeProvider,
    ILogger<CompanyCreatedCapSubscriber> logger)
    : ICapSubscribe
{
    [CapSubscribe(CapTopics.CompanyCreated)]
    public async Task HandleAsync(
        CompanyCreatedIntegrationEvent message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "{Topic} event'i işleniyor. CompanyId: {CompanyId}",
            CapTopics.CompanyCreated,
            message.CompanyId);

        await mediator.Send(
            new CreateCompanySubscriptionCommandRequest
            {
                CompanyId = message.CompanyId,
                PlanId = subscriptionOptions.DefaultPlanId,
                StartDate = timeProvider.GetUtcNow().UtcDateTime
            },
            cancellationToken);
    }
}
