using FluentAssertions;
using Vettingo.AuthService.Domain.Entities;

namespace Vettingo.AuthService.UnitTests;

public sealed class CompanyTests
{
    [Fact]
    public void UpdateCompany_ShouldPreserveRegisteredSubscriberId()
    {
        Guid subscriberId = Guid.NewGuid();
        Company company = new();
        company.RegisterCompany(subscriberId, "Vettingo");

        company.UpdateCompany(
            "Vettingo", "technology", "https://vettingo.com", "51-200",
            "İşe alım platformu", "Maslak, Sarıyer / İstanbul");

        company.Id.Should().Be(subscriberId);
        company.CompanySector.Should().Be("technology");
        company.CompanyWebsite.Should().Be("https://vettingo.com");
        company.CompanySize.Should().Be("51-200");
    }
}
