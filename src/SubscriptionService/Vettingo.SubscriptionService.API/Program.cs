using Vettingo.SubscriptionService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSubscriptionPersistence(builder.Configuration);

var app = builder.Build();

app.Run();
