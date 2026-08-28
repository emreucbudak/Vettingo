using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;
using Vettingo.SubscriptionService.Persistence.Repository;

namespace Vettingo.SubscriptionService.UnitTests.Repository;

public sealed class PlanRepositoryTests
{
    [Fact]
    public async Task Repository_Should_Add_Update_And_Delete_Plan_With_Properties()
    {
        DbContextOptions<SubscriptionDbContext> options =
            new DbContextOptionsBuilder<SubscriptionDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        await using SubscriptionDbContext context = new(options);
        PlanRepository repository = new(context);
        Plan plan = CreatePlan("Starter", 99, "Job postings", 1);

        await repository.AddPlanAsync(plan);
        await repository.SaveChangesAsync();

        plan.Id.Should().BeGreaterThan(0);
        context.ChangeTracker.Clear();

        Plan storedPlan = await repository.GetPlanByIdAsync(plan.Id)
            ?? throw new InvalidOperationException("Kaydedilen plan bulunamadı.");

        storedPlan.PlanProperties.Should().ContainSingle();
        storedPlan.UpdatePlan("Business", 999);
        storedPlan.ReplacePlanProperties(
        [
            CreatePlanProperty("Job postings", 50),
            CreatePlanProperty("Candidate searches", 500)
        ]);

        repository.UpdatePlan(storedPlan);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        Plan updatedPlan = await repository.GetPlanByIdAsync(plan.Id)
            ?? throw new InvalidOperationException("Güncellenen plan bulunamadı.");

        updatedPlan.PlanName.Should().Be("Business");
        updatedPlan.Price.Should().Be(999);
        updatedPlan.PlanProperties.Should().HaveCount(2);

        repository.DeletePlan(updatedPlan);
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
        plan.AddPlanProperty(CreatePlanProperty(propertyName, propertyCount));
        return plan;
    }

    private static PlanProperties CreatePlanProperty(string propertyName, int count)
    {
        PlanProperties planProperty = new();
        planProperty.CreatePlanProperty(propertyName, count);
        return planProperty;
    }
}
