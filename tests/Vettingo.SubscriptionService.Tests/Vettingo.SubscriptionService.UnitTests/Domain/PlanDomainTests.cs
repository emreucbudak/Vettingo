using FluentAssertions;
using Vettingo.SubscriptionService.Domain.Entities;

namespace Vettingo.SubscriptionService.UnitTests.Domain;

public sealed class PlanDomainTests
{
    [Fact]
    public void AddProperty_Should_Create_Property_Inside_Plan_Aggregate()
    {
        Plan plan = new();
        plan.CreatePlan("Business", 999);

        plan.AddProperty("Candidate searches", 500);

        PlanProperties property = plan.PlanProperties.Should().ContainSingle().Subject;
        property.PropertiesName.Should().Be("Candidate searches");
        property.Count.Should().Be(500);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("Job postings", -1)]
    public void AddProperty_Should_Not_Add_Invalid_Property(string propertyName, int count)
    {
        Plan plan = new();
        plan.CreatePlan("Business", 999);

        Action action = () => plan.AddProperty(propertyName, count);

        action.Should().Throw<ArgumentException>();
        plan.PlanProperties.Should().BeEmpty();
    }
}
