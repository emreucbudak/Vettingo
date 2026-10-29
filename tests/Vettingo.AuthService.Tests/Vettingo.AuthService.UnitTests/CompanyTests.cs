using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.AuthService.Application.Repository;
using Vettingo.AuthService.Domain.Entities;
using Vettingo.AuthService.Persistence.Registration;
using AuthDbContext = Vettingo.AuthService.Persistence.DbContext.IdentityDbContext;

namespace Vettingo.AuthService.UnitTests;

public sealed class CompanyTests
{
    [Fact]
    public void Company_Should_Allow_Name_Only_With_Manually_Assigned_Version7_Id()
    {
        Company company = new();
        company.setCompanyName("Vettingo");
        company.SetId();

        company.Id.Version.Should().Be(7);
        company.CompanyName.Should().Be("Vettingo");
        company.CompanySector.Should().BeNull();
        company.CompanyWebsite.Should().BeNull();
        company.CompanySize.Should().BeNull();
        company.CompanyDescription.Should().BeNull();
        company.CompanyAddress.Should().BeNull();
    }

    [Fact]
    public void Company_Model_Should_Keep_Id_Manual_And_User_Relationship_Optional()
    {
        using var context = new AuthDbContext(new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused;Password=unused").Options);
        var company = context.Model.FindEntityType(typeof(Company))!;

        company.FindProperty(nameof(Company.Id))!.ValueGenerated.Should().Be(ValueGenerated.Never);
        company.FindProperty(nameof(Company.CompanyName))!.IsNullable.Should().BeFalse();
        foreach (string property in new[] { nameof(Company.CompanySector), nameof(Company.CompanyWebsite),
            nameof(Company.CompanySize), nameof(Company.CompanyDescription), nameof(Company.CompanyAddress) })
            company.FindProperty(property)!.IsNullable.Should().BeTrue();

        var relation = context.Model.FindEntityType(typeof(User))!.GetForeignKeys()
            .Single(key => key.PrincipalEntityType.ClrType == typeof(Company));
        relation.Properties.Should().ContainSingle().Which.Name.Should().Be(nameof(User.CompanyId));
        relation.IsRequired.Should().BeFalse();
    }

    [Fact]
    public void Company_Repository_Should_Resolve_With_Auth_DbContext()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.SaveDb(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=unused;Username=unused;Password=unused"
        }).Build());
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<ICompanyRepository>().Should().NotBeNull();
    }

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
