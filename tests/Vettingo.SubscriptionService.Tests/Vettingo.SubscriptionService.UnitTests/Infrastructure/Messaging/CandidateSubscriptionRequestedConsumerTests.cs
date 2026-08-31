using FlashMediator;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;
using Vettingo.SubscriptionService.Infrastructure.Messaging;

namespace Vettingo.SubscriptionService.UnitTests.Infrastructure.Messaging;

public sealed class CandidateSubscriptionRequestedConsumerTests
{
    [Fact]
    public async Task HandleAsync_ShouldResolveCandidatePlanAndCreateSubscription()
    {
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Plan plan = new();
        plan.CreatePlan("Pro", 999, PlanType.Candidate);
        planRepository
            .GetPlansByTypeAsync(PlanType.Candidate, Arg.Any<CancellationToken>())
            .Returns([plan]);
        IMediator mediator = Substitute.For<IMediator>();
        mediator
            .Send(Arg.Any<CreateCandidateSubscriptionCommandRequest>(), Arg.Any<CancellationToken>())
            .Returns(Guid.NewGuid());
        CandidateSubscriptionRequestedConsumer consumer = new(
            planRepository,
            mediator,
            NullLogger<CandidateSubscriptionRequestedConsumer>.Instance);
        DateTime startDateUtc = new(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
        CandidateSubscriptionRequestedMessage message = new()
        {
            CandidateId = Guid.NewGuid(),
            PlanCode = "pro",
            BillingPeriod = "annual",
            StartDateUtc = startDateUtc,
            EndDateUtc = startDateUtc.AddYears(1)
        };

        await consumer.HandleAsync(message, CancellationToken.None);

        await planRepository.Received(1).GetPlansByTypeAsync(
            PlanType.Candidate,
            CancellationToken.None);
        await mediator.Received(1).Send(
            Arg.Is<CreateCandidateSubscriptionCommandRequest>(request =>
                request.CandidateId == message.CandidateId &&
                request.PlanId == plan.Id &&
                request.StartDate == message.StartDateUtc &&
                request.EndDate == message.EndDateUtc),
            CancellationToken.None);
    }
}
