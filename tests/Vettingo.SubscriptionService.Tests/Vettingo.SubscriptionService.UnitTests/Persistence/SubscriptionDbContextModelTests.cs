using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Domain.Enums;
using Vettingo.SubscriptionService.Persistence.DbContext;
using Vettingo.SubscriptionService.Persistence.Repository;

namespace Vettingo.SubscriptionService.UnitTests.Persistence;

public sealed class SubscriptionDbContextModelTests
{
    [Fact]
    public void Model_Should_Map_PlanProperties_Through_Backing_Field()
    {
        DbContextOptions<SubscriptionDbContext> options =
            new DbContextOptionsBuilder<SubscriptionDbContext>()
                .UseInMemoryDatabase(nameof(Model_Should_Map_PlanProperties_Through_Backing_Field))
                .Options;
        using SubscriptionDbContext context = new(options);

        var navigation = context.Model
            .FindEntityType(typeof(Plan))
            ?.FindNavigation(nameof(Plan.PlanProperties));

        navigation.Should().NotBeNull();
        navigation!.FieldInfo.Should().NotBeNull();
        navigation.FieldInfo!.Name.Should().Be("_planProperties");
    }

    [Fact]
    public async Task Repository_Should_Filter_Plans_By_Type()
    {
        DbContextOptions<SubscriptionDbContext> options =
            new DbContextOptionsBuilder<SubscriptionDbContext>()
                .UseInMemoryDatabase(nameof(Repository_Should_Filter_Plans_By_Type))
                .Options;
        await using SubscriptionDbContext context = new(options);
        PlanRepository repository = new(context);
        Plan employerPlan = new();
        employerPlan.CreatePlan("Employer Pro", 499, PlanType.Employer);
        Plan candidatePlan = new();
        candidatePlan.CreatePlan("Candidate Pro", 199, PlanType.Candidate);

        await repository.AddPlanAsync(employerPlan);
        await repository.AddPlanAsync(candidatePlan);
        await repository.SaveChangesAsync();

        IReadOnlyList<Plan> candidatePlans =
            await repository.GetPlansByTypeAsync(PlanType.Candidate);

        candidatePlans.Should().ContainSingle();
        candidatePlans[0].PlanName.Should().Be(candidatePlan.PlanName);
        candidatePlans.Should().OnlyContain(plan => plan.PlanType == PlanType.Candidate);
    }
}
