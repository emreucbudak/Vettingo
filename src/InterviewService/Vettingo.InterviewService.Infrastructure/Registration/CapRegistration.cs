using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.InterviewService.Application.Messaging;
using Vettingo.InterviewService.Infrastructure.Messaging;

namespace Vettingo.InterviewService.Infrastructure.Registration;

public static class CapRegistration
{
    public static IServiceCollection AddInterviewCap(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        var rabbitMq = configuration.GetRequiredSection("RabbitMq");
        if (!int.TryParse(rabbitMq["Port"], out var port))
        {
            throw new InvalidOperationException("RabbitMq:Port is not configured or is invalid.");
        }

        services.AddTransient<IInterviewResultPublisher, InterviewResultPublisher>();
        services.AddTransient<InterviewCreateConsumer>();
        services.AddCap(options =>
        {
            options.UsePostgreSql(connectionString);
            options.UseRabbitMQ(rabbitMqOptions =>
            {
                rabbitMqOptions.HostName = rabbitMq.GetRequiredSection("HostName").Value!;
                rabbitMqOptions.Port = port;
                rabbitMqOptions.UserName = rabbitMq.GetRequiredSection("UserName").Value!;
                rabbitMqOptions.Password = rabbitMq.GetRequiredSection("Password").Value!;
                rabbitMqOptions.VirtualHost = rabbitMq.GetRequiredSection("VirtualHost").Value!;
            });
            options.DefaultGroupName = "vettingo.interview-service";
        }).AddSubscriberAssembly(typeof(InterviewCreateConsumer));

        return services;
    }
}
