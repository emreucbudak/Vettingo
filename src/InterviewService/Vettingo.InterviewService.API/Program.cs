using FlashMediator;
using FluentValidation;
using Serilog;
using Vettingo.InterviewService.API.ExceptionHandlers;
using Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion;
using Vettingo.InterviewService.Application.Interfaces;
using Vettingo.InterviewService.Infrastructure.Cache;
using Vettingo.InterviewService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreateInterviewQuestionCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateInterviewQuestionCommandRequest>();
builder.Services.AddDistributedMemoryCache();
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
