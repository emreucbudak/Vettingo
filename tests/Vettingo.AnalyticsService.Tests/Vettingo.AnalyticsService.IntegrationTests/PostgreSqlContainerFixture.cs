using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.AnalyticsService.Persistence.DbContext;

namespace Vettingo.AnalyticsService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("AnalyticsServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public AnalyticsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseNpgsql(_postgreSqlContainer.GetConnectionString())
            .Options;

        return new AnalyticsDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using var context = CreateDbContext();
        if (context.Database.GetMigrations().Any())
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }
}
