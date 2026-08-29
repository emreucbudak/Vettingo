using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.Subscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.UnitTests.Application.CQRS;

public sealed class SubscriptionCqrsTests
{
    [Fact]
    public async Task CreateCompanySubscriptionCommandHandler_Should_Create_Subscription_And_Save()
    {
        ISubscriptionRepository subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Plan plan = new();
        plan.CreatePlan("Starter", 0);
        planRepository.GetPlanByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(plan));
        subscriptionRepository
            .GetSubscriptionsByCompanyIdAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Subscription>());
        CreateCompanySubscriptionCommandHandler handler = new(
            subscriptionRepository,
            planRepository,
            Substitute.For<ILogger<CreateCompanySubscriptionCommandHandler>>());
        DateTime startDate = new(2026, 8, 29, 12, 0, 0, DateTimeKind.Utc);
        CreateCompanySubscriptionCommandRequest request = new()
        {
            CompanyId = Guid.NewGuid(),
            PlanId = 1,
            StartDate = startDate
        };

        Guid subscriptionId = await handler.Handle(request, CancellationToken.None);

        subscriptionId.Should().NotBeEmpty();
        await subscriptionRepository.Received(1).AddSubscriptionAsync(
            Arg.Is<Subscription>(subscription =>
                subscription.Id == subscriptionId &&
                subscription.CompanyId == request.CompanyId &&
                subscription.PlanId == request.PlanId &&
                subscription.StartDate == startDate &&
                subscription.EndDate == null),
            CancellationToken.None);
        await subscriptionRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CreateCompanySubscriptionCommandHandler_Should_Return_Existing_Subscription()
    {
        ISubscriptionRepository subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Guid companyId = Guid.NewGuid();
        Subscription existingSubscription = new();
        existingSubscription.CreateSubscription(
            companyId,
            1,
            DateTime.UtcNow,
            null);
        subscriptionRepository
            .GetSubscriptionsByCompanyIdAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(new[] { existingSubscription });
        CreateCompanySubscriptionCommandHandler handler = new(
            subscriptionRepository,
            planRepository,
            Substitute.For<ILogger<CreateCompanySubscriptionCommandHandler>>());

        Guid subscriptionId = await handler.Handle(
            new CreateCompanySubscriptionCommandRequest
            {
                CompanyId = companyId,
                PlanId = 1,
                StartDate = DateTime.UtcNow
            },
            CancellationToken.None);

        subscriptionId.Should().Be(existingSubscription.Id);
        await planRepository.DidNotReceiveWithAnyArgs()
            .GetPlanByIdAsync(default, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .AddSubscriptionAsync(default!, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(default);
    }

    [Fact]
    public async Task CreateCompanySubscriptionCommandHandler_Should_Throw_When_Plan_Does_Not_Exist()
    {
        ISubscriptionRepository subscriptionRepository = Substitute.For<ISubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Guid companyId = Guid.NewGuid();
        subscriptionRepository
            .GetSubscriptionsByCompanyIdAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Subscription>());
        planRepository.GetPlanByIdAsync(42, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(null));
        CreateCompanySubscriptionCommandHandler handler = new(
            subscriptionRepository,
            planRepository,
            Substitute.For<ILogger<CreateCompanySubscriptionCommandHandler>>());

        Func<Task> action = () => handler.Handle(
            new CreateCompanySubscriptionCommandRequest
            {
                CompanyId = companyId,
                PlanId = 42,
                StartDate = DateTime.UtcNow
            },
            CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .AddSubscriptionAsync(default!, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(default);
    }
}
