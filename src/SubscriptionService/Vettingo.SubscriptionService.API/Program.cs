using FlashMediator;
using FluentValidation;
using Vettingo.SubscriptionService.API.ExceptionHandlers;
using Vettingo.SubscriptionService.Application.Features.CQRS.Plan.Command.CreatePlan;
using Vettingo.SubscriptionService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSubscriptionPersistence(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreatePlanCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreatePlanCommandRequest>();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<DomainValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();

public partial class Program
{
}
