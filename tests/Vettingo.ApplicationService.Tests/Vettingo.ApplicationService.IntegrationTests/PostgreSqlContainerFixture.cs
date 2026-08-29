using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.ApplicationService.Persistence.DbContext;

namespace Vettingo.ApplicationService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("ApplicationServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public ApplicationDbContext CreateDbContext()
    {
        DbContextOptions<ApplicationDbContext> options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

        return new ApplicationDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using ApplicationDbContext dbContext = CreateDbContext();
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
