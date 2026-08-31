using FlashMediator;
using FluentValidation;
using Vettingo.SubscriptionService.API.ExceptionHandlers;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;
using Vettingo.SubscriptionService.Application.Services;
using Vettingo.SubscriptionService.Infrastructure.Register;
using Vettingo.SubscriptionService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSubscriptionPersistence(builder.Configuration);
builder.Services.AddSubscriptionCap(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreatePlanCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreatePlanCommandRequest>();
builder.Services.AddScoped<ISubscriptionActivationService, SubscriptionActivationService>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException(
            "Connection string 'Redis' is not configured.");
    options.InstanceName = "Vettingo:SubscriptionService:";
});
builder.Services.AddStripePayments(builder.Configuration);

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<DomainValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();
