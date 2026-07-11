using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Persistence.DbContext;
using Vettingo.EvaluationService.Persistence.Repository;

namespace Vettingo.EvaluationService.Persistence.Registration;

public static class DbContextRegistration
{
    public static IServiceCollection AddEvaluationPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection bağlantı dizesi tanımlı değil.");

        services.AddDbContext<EvaluationDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();

        return services;
    }
}
