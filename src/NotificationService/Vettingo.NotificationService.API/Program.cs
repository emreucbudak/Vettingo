using Serilog;
using FlashMediator;
using FluentValidation;
using Vettingo.NotificationService.API.ExceptionHandlers;
using Vettingo.NotificationService.API.Middleware;
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
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddTransient<RedisCacheMiddleware>();
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
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} isteği {StatusCode} durum koduyla {Elapsed:0.0000} ms içinde tamamlandı";
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseExceptionHandler();

app.UseAuthorization();

app.UseMiddleware<RedisCacheMiddleware>();

app.MapControllers();
app.MapHub<NotificationHub>("/notification-hub");

app.Run();

