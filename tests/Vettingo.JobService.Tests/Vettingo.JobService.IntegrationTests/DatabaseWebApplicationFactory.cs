using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace Vettingo.JobService.IntegrationTests;

public sealed class DatabaseWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
    }
}
