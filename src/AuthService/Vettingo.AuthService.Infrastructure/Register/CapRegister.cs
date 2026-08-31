using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vettingo.AuthService.Infrastructure.Messaging;

namespace Vettingo.AuthService.Infrastructure.Register
{
    public static class CapRegister
    {
        public static void AddCapServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<SubscriptionRegistrationRequestedConsumer>();

            string databaseConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
            string rabbitMqHostName = configuration["RabbitMq:HostName"]
                ?? throw new InvalidOperationException("RabbitMq:HostName is not configured.");
            string rabbitMqUserName = configuration["RabbitMq:UserName"]
                ?? throw new InvalidOperationException("RabbitMq:UserName is not configured.");
            string rabbitMqPassword = configuration["RabbitMq:Password"]
                ?? throw new InvalidOperationException("RabbitMq:Password is not configured.");
            string rabbitMqVirtualHost = configuration["RabbitMq:VirtualHost"]
                ?? throw new InvalidOperationException("RabbitMq:VirtualHost is not configured.");

            if (!int.TryParse(configuration["RabbitMq:Port"], out int rabbitMqPort))
            {
                throw new InvalidOperationException("RabbitMq:Port is not configured or is invalid.");
            }

            services
                .AddCap(options =>
            {
                options.UsePostgreSql(databaseConnectionString);
                options.UseRabbitMQ(rabbitMqOptions =>
                {
                    rabbitMqOptions.HostName = rabbitMqHostName;
                    rabbitMqOptions.Port = rabbitMqPort;
                    rabbitMqOptions.UserName = rabbitMqUserName;
                    rabbitMqOptions.Password = rabbitMqPassword;
                    rabbitMqOptions.VirtualHost = rabbitMqVirtualHost;
                });

                options.DefaultGroupName = "vettingo.auth-service";
                options.SucceedMessageExpiredAfter = 24 * 60 * 60;
                options.FailedMessageExpiredAfter = 15 * 24 * 60 * 60;
                options.FailedRetryInterval = 60;
                options.ConsumerThreadCount = 3;
                options.FailedRetryCount = 5;
            })
                .AddSubscriberAssembly(
                    typeof(SubscriptionRegistrationRequestedConsumer));
        }
    }
}
