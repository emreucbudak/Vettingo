using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.SubscriptionService.Domain.Entities;
using Vettingo.SubscriptionService.Persistence.DbContext;

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
}
