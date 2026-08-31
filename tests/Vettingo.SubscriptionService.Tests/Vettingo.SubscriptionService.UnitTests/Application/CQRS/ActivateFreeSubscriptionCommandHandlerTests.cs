using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Reflection;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.Payment.Command.ActivateFreeSubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Application.Services;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.UnitTests.Application.CQRS;

public sealed class ActivateFreeSubscriptionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldActivateCandidateSubscriptionAndRequestAuthRegistration()
    {
        Guid registrationToken = Guid.NewGuid();
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        ActivateFreeSubscriptionCommandHandler handler = new(
            CreatePlanRepository(1, PlanType.Candidate, 0),
            CreateCache(),
            activationService);

        ActivateFreeSubscriptionCommandResponse response = await handler.Handle(
            new ActivateFreeSubscriptionCommandRequest
            {
                AccountType = "Candidate",
                BillingPeriod = "monthly",
                PlanId = 1,
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        response.Completed.Should().BeTrue();
        await activationService.Received(1).ActivateAsync(
            "candidate",
            Arg.Is<Guid>(subscriberId => subscriberId != Guid.Empty),
            1,
            "monthly",
            registrationToken,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReuseCachedSubscriberId()
    {
        Guid registrationToken = Guid.NewGuid();
        Guid subscriberId = Guid.NewGuid();
        IDistributedCache cache = CreateCache();
        cache
            .GetAsync(
                $"free-subscription:employer:{registrationToken:D}:2:annual",
                Arg.Any<CancellationToken>())
            .Returns(System.Text.Encoding.UTF8.GetBytes(subscriberId.ToString("D")));
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        ActivateFreeSubscriptionCommandHandler handler = new(
            CreatePlanRepository(2, PlanType.Employer, 0),
            cache,
            activationService);

        await handler.Handle(
            new ActivateFreeSubscriptionCommandRequest
            {
                AccountType = "employer",
                BillingPeriod = "Annual",
                PlanId = 2,
                RegistrationToken = registrationToken
            },
            CancellationToken.None);

        await activationService.Received(1).ActivateAsync(
            "employer",
            subscriberId,
            2,
            "annual",
            registrationToken,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldRejectPaidPlanWithoutActivation()
    {
        ISubscriptionActivationService activationService =
            Substitute.For<ISubscriptionActivationService>();
        ActivateFreeSubscriptionCommandHandler handler = new(
            CreatePlanRepository(3, PlanType.Candidate, 10),
            CreateCache(),
            activationService);

        Func<Task> action = () => handler.Handle(
            new ActivateFreeSubscriptionCommandRequest
            {
                AccountType = "candidate",
                BillingPeriod = "monthly",
                PlanId = 3,
                RegistrationToken = Guid.NewGuid()
            },
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("*fiyatı sıfır*");
        await activationService.DidNotReceiveWithAnyArgs().ActivateAsync(
            default!,
            default,
            default,
            default!,
            default,
            default);
    }

    private static IDistributedCache CreateCache()
    {
        IDistributedCache cache = Substitute.For<IDistributedCache>();
        cache
            .SetAsync(
                Arg.Any<string>(),
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        return cache;
    }

    private static IPlanRepository CreatePlanRepository(
        int planId,
        PlanType planType,
        int price)
    {
        Plan plan = new();
        plan.CreatePlan("Plan", price, planType);
        typeof(Plan)
            .GetProperty(nameof(Plan.Id), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(plan, planId);

        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        planRepository
            .GetPlanByIdAsync(planId, Arg.Any<CancellationToken>())
            .Returns(plan);
        return planRepository;
    }
}
