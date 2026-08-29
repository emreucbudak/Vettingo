using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.SubscriptionService.Persistence.DbContext;

namespace Vettingo.SubscriptionService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("SubscriptionServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public SubscriptionDbContext CreateDbContext()
    {
        DbContextOptions<SubscriptionDbContext> options =
            new DbContextOptionsBuilder<SubscriptionDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

        return new SubscriptionDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using SubscriptionDbContext dbContext = CreateDbContext();
        if (dbContext.Database.GetMigrations().Any())
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}
