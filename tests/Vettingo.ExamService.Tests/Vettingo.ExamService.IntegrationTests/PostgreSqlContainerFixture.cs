using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Vettingo.ExamService.Persistence.DbContext;

namespace Vettingo.ExamService.IntegrationTests;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("ExamServiceDb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public ExamDbContext CreateDbContext()
    {
        DbContextOptions<ExamDbContext> options =
            new DbContextOptionsBuilder<ExamDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

        return new ExamDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        await using ExamDbContext dbContext = CreateDbContext();
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
