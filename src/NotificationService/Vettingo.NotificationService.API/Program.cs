using FlashMediator;
using FluentValidation;
using Vettingo.NotificationService.Application.Features.CQRS.Notification.Command.CreateNotification;
using Vettingo.NotificationService.Infrastructure.Hubs;
using Vettingo.NotificationService.Infrastructure.Registration;
using Vettingo.NotificationService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddNotificationInfrastructure();
builder.Services.AddFlashMediator(typeof(CreateNotificationCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateNotificationCommandRequest>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notification-hub");

app.Run();
