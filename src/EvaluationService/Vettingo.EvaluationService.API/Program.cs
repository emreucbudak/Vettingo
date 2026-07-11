using FlashMediator;
using FluentValidation;
using Serilog;
using Vettingo.EvaluationService.API.ExceptionHandlers;
using Vettingo.EvaluationService.API.Pipeline;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.CreateEvaluation;
using Vettingo.EvaluationService.Application.Interfaces;
using Vettingo.EvaluationService.Infrastructure.Cache;
using Vettingo.EvaluationService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddEvaluationPersistence(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreateEvaluationCommandHandler).Assembly);
builder.Services.AddPipelineBehavior(typeof(RedisCachePipelineBehaviour<,>));
builder.Services.AddValidatorsFromAssemblyContaining<CreateEvaluationCommandRequest>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<DomainValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BaseExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} isteği {StatusCode} durum koduyla {Elapsed:0.0000} ms içinde tamamlandı";
});

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();
