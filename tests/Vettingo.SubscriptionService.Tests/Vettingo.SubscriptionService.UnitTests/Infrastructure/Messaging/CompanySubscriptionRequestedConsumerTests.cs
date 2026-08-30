using FlashMediator;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;
using Vettingo.SubscriptionService.Infrastructure.Messaging;

namespace Vettingo.SubscriptionService.UnitTests.Infrastructure.Messaging;

public sealed class CompanySubscriptionRequestedConsumerTests
{
    [Fact]
    public async Task HandleAsync_ShouldResolveEmployerPlanAndCreateSubscription()
    {
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Plan plan = new();
        plan.CreatePlan("Pro", 2999, PlanType.Employer);
        planRepository
            .GetPlansByTypeAsync(PlanType.Employer, Arg.Any<CancellationToken>())
            .Returns([plan]);
        IMediator mediator = Substitute.For<IMediator>();
        mediator
            .Send(Arg.Any<CreateCompanySubscriptionCommandRequest>(), Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());
        CompanySubscriptionRequestedConsumer consumer = new(
            planRepository,
            mediator,
            NullLogger<CompanySubscriptionRequestedConsumer>.Instance);
        DateTime startDateUtc = new(2026, 8, 30, 12, 0, 0, DateTimeKind.Utc);
        CompanySubscriptionRequestedMessage message = new()
        {
            CompanyId = Guid.NewGuid(),
            PlanCode = "pro",
            BillingPeriod = "annual",
            StartDateUtc = startDateUtc,
            EndDateUtc = startDateUtc.AddYears(1)
        };

        await consumer.HandleAsync(message, CancellationToken.None);

        await mediator.Received(1).Send(
            Arg.Is<CreateCompanySubscriptionCommandRequest>(request =>
                request.CompanyId == message.CompanyId &&
                request.PlanId == plan.Id &&
                request.StartDate == message.StartDateUtc &&
                request.EndDate == message.EndDateUtc),
            CancellationToken.None);
    }
}
