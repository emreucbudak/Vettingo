using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.InterviewService.Persistence.DbContext;

namespace Vettingo.InterviewService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("InterviewServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public InterviewDbContext CreateDbContext()
    {
        DbContextOptions<InterviewDbContext> options =
            new DbContextOptionsBuilder<InterviewDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

        return new InterviewDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using InterviewDbContext dbContext = CreateDbContext();
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
