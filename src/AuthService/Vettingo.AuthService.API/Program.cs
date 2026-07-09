using Vettingo.AuthService.Application.Features.CQRS.Auth.Command.Register;
using Serilog;
using FluentValidation;
using Vettingo.AuthService.API.ExceptionHandlers;
using Vettingo.AuthService.Infrastructure.Register;
using Vettingo.AuthService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandRequest>();
builder.Services.AddTokenSettings(builder.Configuration);
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

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();

