using FluentAssertions;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;
using Vettingo.SubscriptionService.IntegrationTests;
using Vettingo.SubscriptionService.Persistence.DbContext;
using Vettingo.SubscriptionService.Persistence.Repository;

namespace Vettingo.SubscriptionService.UnitTests.Repository;

public sealed class PlanRepositoryTests : IClassFixture<PostgreSqlContainerFixture>
{
    private readonly PostgreSqlContainerFixture _fixture;

    public PlanRepositoryTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Repository_Should_Add_Update_And_Delete_Plan_With_Properties()
    {
        await using SubscriptionDbContext context = _fixture.CreateDbContext();
        PlanRepository repository = new(context);
        Plan plan = CreatePlan("Starter", 99, "Job postings", 1);
        Plan candidatePlan = new();
        candidatePlan.CreatePlan("Candidate Pro", 199, PlanType.Candidate);

        await repository.AddPlanAsync(plan);
        await repository.AddPlanAsync(candidatePlan);
        await repository.SaveChangesAsync();

        plan.Id.Should().BeGreaterThan(0);
        candidatePlan.Id.Should().BeGreaterThan(0);
        context.ChangeTracker.Clear();

        IReadOnlyList<Plan> candidatePlans =
            await repository.GetPlansByTypeAsync(PlanType.Candidate);
        candidatePlans.Should().Contain(candidate => candidate.Id == candidatePlan.Id);
        candidatePlans.Should().OnlyContain(candidate => candidate.PlanType == PlanType.Candidate);

        Plan storedPlan = await repository.GetPlanByIdAsync(plan.Id)
            ?? throw new InvalidOperationException("Kaydedilen plan bulunamadı.");

        storedPlan.PlanProperties.Should().ContainSingle();
        storedPlan.UpdatePlan("Business", 999);
        storedPlan.AddProperty("Candidate searches", 500);

        repository.UpdatePlan(storedPlan);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        Plan updatedPlan = await repository.GetPlanByIdAsync(plan.Id)
            ?? throw new InvalidOperationException("Güncellenen plan bulunamadı.");

        updatedPlan.PlanName.Should().Be("Business");
        updatedPlan.Price.Should().Be(999);
        updatedPlan.PlanProperties.Should().HaveCount(2);
        updatedPlan.PlanProperties.Should().Contain(property =>
            property.PropertiesName == "Candidate searches" &&
            property.Count == 500);

        repository.DeletePlan(updatedPlan);
        Plan storedCandidatePlan = await repository.GetPlanByIdAsync(candidatePlan.Id)
            ?? throw new InvalidOperationException("Aday planı bulunamadı.");
        repository.DeletePlan(storedCandidatePlan);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        (await repository.GetPlanByIdAsync(plan.Id)).Should().BeNull();
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
