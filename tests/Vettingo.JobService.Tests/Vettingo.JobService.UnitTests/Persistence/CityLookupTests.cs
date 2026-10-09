using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Application.Features.CQRS.City.Query.GetAll;
using Vettingo.JobService.Domain.Entities;
using Vettingo.JobService.Persistence.DbContext;
using Vettingo.JobService.Persistence.Repository;

namespace Vettingo.JobService.UnitTests.Persistence;

public sealed class CityLookupTests
{
    [Fact]
    public async Task List_Should_Return_Seeded_Cities_And_Reflect_Database_Changes()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        context.ChangeTracker.Clear();
        var handler = new GetAllCitiesQueryHandler(new CityRepository(context));

        var cities = await handler.Handle(new(), CancellationToken.None);

        cities.Should().HaveCount(81);
        cities.Select(city => city.Id).Should().Equal(Enumerable.Range(1, 81));
        cities.Single(city => city.Id == 34).CityName.Should().Be("İstanbul");
        cities.Should().OnlyContain(city => city.CountryCode == "TR");
        context.ChangeTracker.Entries().Should().BeEmpty();

        var cityToUpdate = await context.Cities.SingleAsync(city => city.Id == 34);
        cityToUpdate.CityName = "İstanbul güncel";
        context.Cities.Add(new City { Id = 82, CityName = "Test Şehri", CountryCode = "TR" });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var updatedCities = await handler.Handle(new(), CancellationToken.None);
        updatedCities.Should().HaveCount(82);
        updatedCities.Single(city => city.Id == 34).CityName.Should().Be("İstanbul güncel");
        updatedCities.Single(city => city.Id == 82).CityName.Should().Be("Test Şehri");
        context.ChangeTracker.Entries().Should().BeEmpty();
    }

    [Fact]
    public async Task List_Should_Return_Empty_When_City_Table_Is_Empty()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
        context.Cities.RemoveRange(context.Cities);
        await context.SaveChangesAsync();

        var cities = await new GetAllCitiesQueryHandler(new CityRepository(context))
            .Handle(new(), CancellationToken.None);

        cities.Should().BeEmpty();
    }

    private static JobDbContext CreateContext() => new(
        new DbContextOptionsBuilder<JobDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
