using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.NotificationService.Persistence.DbContext;

namespace Vettingo.NotificationService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("NotificationServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public NotificationDbContext CreateDbContext()
    {
        DbContextOptions<NotificationDbContext> options =
            new DbContextOptionsBuilder<NotificationDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

        return new NotificationDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using NotificationDbContext dbContext = CreateDbContext();
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
