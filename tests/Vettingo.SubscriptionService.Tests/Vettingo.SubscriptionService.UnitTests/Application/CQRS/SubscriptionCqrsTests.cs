using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Exceptions;
using Vettingo.SubscriptionService.Application.Features.CQRS.CandidateSubscription.Command.CreateCandidateSubscription;
using Vettingo.SubscriptionService.Application.Features.CQRS.CompanySubscription.Command.CreateCompanySubscription;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.UnitTests.Application.CQRS;

public sealed class SubscriptionCqrsTests
{
    [Fact]
    public async Task CreateCompanySubscriptionCommandHandler_Should_Create_Subscription_And_Save()
    {
        ICompanySubscriptionRepository subscriptionRepository = Substitute.For<ICompanySubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Plan plan = new();
        plan.CreatePlan("Starter", 0);
        planRepository.GetPlanByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(plan));
        subscriptionRepository
            .GetCompanySubscriptionsByCompanyIdAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<CompanySubscription>());
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
        await subscriptionRepository.Received(1).AddCompanySubscriptionAsync(
            Arg.Is<CompanySubscription>(subscription =>
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
        ICompanySubscriptionRepository subscriptionRepository = Substitute.For<ICompanySubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Guid companyId = Guid.NewGuid();
        CompanySubscription existingSubscription = new();
        existingSubscription.CreateCompanySubscription(
            companyId,
            1,
            DateTime.UtcNow,
            null);
        subscriptionRepository
            .GetCompanySubscriptionsByCompanyIdAsync(companyId, Arg.Any<CancellationToken>())
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
            .AddCompanySubscriptionAsync(default!, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(default);
    }

    [Fact]
    public async Task CreateCompanySubscriptionCommandHandler_Should_Throw_When_Plan_Does_Not_Exist()
    {
        ICompanySubscriptionRepository subscriptionRepository = Substitute.For<ICompanySubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Guid companyId = Guid.NewGuid();
        subscriptionRepository
            .GetCompanySubscriptionsByCompanyIdAsync(companyId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<CompanySubscription>());
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
            .AddCompanySubscriptionAsync(default!, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(default);
    }

    [Fact]
    public async Task CreateCandidateSubscriptionCommandHandler_Should_Create_Subscription_And_Save()
    {
        ICandidateSubscriptionRepository subscriptionRepository =
            Substitute.For<ICandidateSubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Plan plan = new();
        plan.CreatePlan("Pro", 999, PlanType.Candidate);
        planRepository.GetPlanByIdAsync(2, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(plan));
        subscriptionRepository
            .GetCandidateSubscriptionsByCandidateIdAsync(
                Arg.Any<Guid>(),
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<CandidateSubscription>());
        CreateCandidateSubscriptionCommandHandler handler = new(
            subscriptionRepository,
            planRepository,
            Substitute.For<ILogger<CreateCandidateSubscriptionCommandHandler>>());
        DateTime startDate = new(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
        CreateCandidateSubscriptionCommandRequest request = new()
        {
            CandidateId = Guid.NewGuid(),
            PlanId = 2,
            StartDate = startDate,
            EndDate = startDate.AddMonths(1)
        };

        Guid subscriptionId = await handler.Handle(request, CancellationToken.None);

        subscriptionId.Should().NotBeEmpty();
        await subscriptionRepository.Received(1).AddCandidateSubscriptionAsync(
            Arg.Is<CandidateSubscription>(subscription =>
                subscription.Id == subscriptionId &&
                subscription.CandidateId == request.CandidateId &&
                subscription.PlanId == request.PlanId &&
                subscription.StartDate == request.StartDate &&
                subscription.EndDate == request.EndDate),
            CancellationToken.None);
        await subscriptionRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task CreateCandidateSubscriptionCommandHandler_Should_Return_Existing_Subscription()
    {
        ICandidateSubscriptionRepository subscriptionRepository =
            Substitute.For<ICandidateSubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Guid candidateId = Guid.NewGuid();
        CandidateSubscription existingSubscription = new();
        existingSubscription.CreateCandidateSubscription(
            candidateId,
            2,
            DateTime.UtcNow,
            null);
        subscriptionRepository
            .GetCandidateSubscriptionsByCandidateIdAsync(
                candidateId,
                Arg.Any<CancellationToken>())
            .Returns([existingSubscription]);
        CreateCandidateSubscriptionCommandHandler handler = new(
            subscriptionRepository,
            planRepository,
            Substitute.For<ILogger<CreateCandidateSubscriptionCommandHandler>>());

        Guid subscriptionId = await handler.Handle(
            new CreateCandidateSubscriptionCommandRequest
            {
                CandidateId = candidateId,
                PlanId = 2,
                StartDate = DateTime.UtcNow
            },
            CancellationToken.None);

        subscriptionId.Should().Be(existingSubscription.Id);
        await planRepository.DidNotReceiveWithAnyArgs()
            .GetPlanByIdAsync(default, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .AddCandidateSubscriptionAsync(default!, default);
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(default);
    }

    [Fact]
    public async Task CreateCandidateSubscriptionCommandHandler_Should_Reject_Employer_Plan()
    {
        ICandidateSubscriptionRepository subscriptionRepository =
            Substitute.For<ICandidateSubscriptionRepository>();
        IPlanRepository planRepository = Substitute.For<IPlanRepository>();
        Guid candidateId = Guid.NewGuid();
        Plan employerPlan = new();
        employerPlan.CreatePlan("Pro", 2999, PlanType.Employer);
        subscriptionRepository
            .GetCandidateSubscriptionsByCandidateIdAsync(
                candidateId,
                Arg.Any<CancellationToken>())
            .Returns(Array.Empty<CandidateSubscription>());
        planRepository.GetPlanByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(employerPlan));
        CreateCandidateSubscriptionCommandHandler handler = new(
            subscriptionRepository,
            planRepository,
            Substitute.For<ILogger<CreateCandidateSubscriptionCommandHandler>>());

        Func<Task> action = () => handler.Handle(
            new CreateCandidateSubscriptionCommandRequest
            {
                CandidateId = candidateId,
                PlanId = 1,
                StartDate = DateTime.UtcNow
            },
            CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await subscriptionRepository.DidNotReceiveWithAnyArgs()
            .AddCandidateSubscriptionAsync(default!, default);
    }
}
