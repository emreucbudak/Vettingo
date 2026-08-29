using FlashMediator;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Infrastructure.Messaging;
using Vettingo.SubscriptionService.Infrastructure.Options;
using Vettingo.SubscriptionService.Infrastructure.Subscribers;

namespace Vettingo.SubscriptionService.UnitTests.Infrastructure.Subscribers;

public sealed class CompanyCreatedCapSubscriberTests
{
    [Fact]
    public async Task HandleAsync_Should_Send_Create_Subscription_Command_To_Mediator()
    {
        IMediator mediator = Substitute.For<IMediator>();
        DateTimeOffset now = new(2026, 8, 29, 12, 0, 0, TimeSpan.Zero);
        CompanyCreatedCapSubscriber subscriber = new(
            mediator,
            new CompanySubscriptionOptions { DefaultPlanId = 7 },
            new FixedTimeProvider(now),
            Substitute.For<ILogger<CompanyCreatedCapSubscriber>>());
        CompanyCreatedIntegrationEvent message = new()
        {
            CompanyId = Guid.NewGuid()
        };

        await subscriber.HandleAsync(message, CancellationToken.None);

        await mediator.Received(1).Send(
            Arg.Is<CreateCompanySubscriptionCommandRequest>(request =>
                request.CompanyId == message.CompanyId &&
                request.PlanId == 7 &&
                request.StartDate == now.UtcDateTime &&
                request.EndDate == null),
            CancellationToken.None);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
