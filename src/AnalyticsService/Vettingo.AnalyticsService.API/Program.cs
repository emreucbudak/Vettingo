using Serilog;
using FlashMediator;
using FluentValidation;
using Vettingo.AnalyticsService.API.ExceptionHandlers;
using Vettingo.AnalyticsService.Application.Features.CQRS.Analytics.Command.RecordCandidateRecommendation;
using Vettingo.AnalyticsService.Application.Interfaces;
using Vettingo.AnalyticsService.Infrastructure.Cache;
using Vettingo.AnalyticsService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddFlashMediator(typeof(RecordCandidateRecommendationCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<RecordCandidateRecommendationCommandRequest>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");
    options.InstanceName = "Vettingo:AnalyticsService:";
});
builder.Services.AddScoped<ICacheService, CacheService>();
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

app.UseExceptionHandler();

app.UseRouting();

app.UseAuthorization();



app.MapControllers();

app.Run();

