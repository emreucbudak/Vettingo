using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.DeletePlan;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.UpdatePlan;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Query.GetByType;
using Vettingo.SubscriptionService.Application.Repository;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;

namespace Vettingo.SubscriptionService.UnitTests.Application.CQRS;

public sealed class PlanCqrsTests
{
    [Fact]
    public async Task CreatePlanCommandHandler_Should_Create_Plan_And_Save()
    {
        IPlanRepository repository = Substitute.For<IPlanRepository>();
        repository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        CreatePlanCommandHandler handler = new(
            repository,
            Substitute.For<ILogger<CreatePlanCommandHandler>>());
        CreatePlanCommandRequest request = new()
        {
            PlanName = "Professional",
            Price = 499,
            PlanType = PlanType.Candidate
        };

        await handler.Handle(request, CancellationToken.None);

        await repository.Received(1).AddPlanAsync(
            Arg.Is<Plan>(plan =>
                plan.PlanName == request.PlanName &&
                plan.Price == request.Price &&
                plan.PlanType == request.PlanType &&
                plan.PlanProperties.Count == 0),
            CancellationToken.None);
        await repository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task UpdatePlanCommandHandler_Should_Update_Plan_And_Preserve_Properties()
    {
        IPlanRepository repository = Substitute.For<IPlanRepository>();
        Plan plan = CreatePlan("Starter", 99, "Job postings", 1);
        repository.GetPlanByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(plan));
        repository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        UpdatePlanCommandHandler handler = new(
            repository,
            Substitute.For<ILogger<UpdatePlanCommandHandler>>());
        UpdatePlanCommandRequest request = new()
        {
            PlanId = 1,
            PlanName = "Business",
            Price = 999,
            PlanType = PlanType.Candidate
        };

        await handler.Handle(request, CancellationToken.None);

        plan.PlanName.Should().Be(request.PlanName);
        plan.Price.Should().Be(request.Price);
        plan.PlanType.Should().Be(PlanType.Candidate);
        plan.PlanProperties.Should().ContainSingle();
        plan.PlanProperties.Single().Count.Should().Be(1);
        repository.Received(1).UpdatePlan(plan);
        await repository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task GetPlansByTypeQueryHandler_Should_Return_Only_Requested_Plan_Type()
    {
        IPlanRepository repository = Substitute.For<IPlanRepository>();
        Plan plan = CreatePlan("Candidate Pro", 199, "Applications", 25);
        plan.UpdatePlan(plan.PlanName, plan.Price, PlanType.Candidate);
        repository
            .GetPlansByTypeAsync(PlanType.Candidate, Arg.Any<CancellationToken>())
            .Returns([plan]);
        GetPlansByTypeQueryHandler handler = new(
            repository,
            Substitute.For<ILogger<GetPlansByTypeQueryHandler>>());

        IReadOnlyList<GetPlansByTypeQueryResponse> response = await handler.Handle(
            new GetPlansByTypeQueryRequest { PlanType = PlanType.Candidate },
            CancellationToken.None);

        response.Should().ContainSingle();
        response[0].PlanType.Should().Be(PlanType.Candidate);
        response[0].PlanName.Should().Be(plan.PlanName);
        response[0].Properties.Should().ContainSingle();
        await repository.Received(1).GetPlansByTypeAsync(
            PlanType.Candidate,
            CancellationToken.None);
    }

    [Fact]
    public async Task DeletePlanCommandHandler_Should_Delete_Plan_And_Save()
    {
        IPlanRepository repository = Substitute.For<IPlanRepository>();
        Plan plan = CreatePlan("Starter", 99, "Job postings", 1);
        repository.GetPlanByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Plan?>(plan));
        repository.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        DeletePlanCommandHandler handler = new(
            repository,
            Substitute.For<ILogger<DeletePlanCommandHandler>>());

        await handler.Handle(
            new DeletePlanCommandRequest { PlanId = 1 },
            CancellationToken.None);

        repository.Received(1).DeletePlan(plan);
        await repository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    private static Plan CreatePlan(
        string planName,
        int price,
        string propertyName,
        int propertyCount)
    {
        Plan plan = new();
        plan.CreatePlan(planName, price);
        plan.AddProperty(propertyName, propertyCount);

        return plan;
    }
}
