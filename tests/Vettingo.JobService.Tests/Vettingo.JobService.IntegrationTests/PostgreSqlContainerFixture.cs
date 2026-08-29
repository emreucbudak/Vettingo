using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.JobService.Persistence.DbContext;

namespace Vettingo.JobService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("JobServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public JobDbContext CreateDbContext()
    {
        DbContextOptions<JobDbContext> options =
            new DbContextOptionsBuilder<JobDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

        return new JobDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using JobDbContext dbContext = CreateDbContext();
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
