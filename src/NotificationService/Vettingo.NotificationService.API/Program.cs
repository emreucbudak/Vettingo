using Serilog;
using FlashMediator;
using FluentValidation;
using Vettingo.NotificationService.API.ExceptionHandlers;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Application.Interfaces;
using Vettingo.NotificationService.Infrastructure.Cache;
using Vettingo.NotificationService.Infrastructure.Hubs;
using Vettingo.NotificationService.Infrastructure.Registration;
using Vettingo.NotificationService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");
    options.InstanceName = "Vettingo:NotificationService:";
});
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddSignalR();
builder.Services.AddNotificationInfrastructure();
builder.Services.AddFlashMediator(typeof(CreateNotificationCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateNotificationCommandRequest>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<BaseExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();


var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} isteği {StatusCode} durum koduyla {Elapsed:0.0000} ms içinde tamamlandı";
});



app.UseHttpsRedirection();

app.UseRouting();

app.UseExceptionHandler();

app.UseAuthorization();


app.MapControllers();
app.MapHub<NotificationHub>("/notification-hub");

app.Run();

