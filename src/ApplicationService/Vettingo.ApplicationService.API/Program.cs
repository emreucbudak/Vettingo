using System.Text.Json.Serialization;
using FlashMediator;
using FluentValidation;
using Serilog;
using Vettingo.ApplicationService.API.ExceptionHandlers;
using Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Command.Create;
using Vettingo.ApplicationService.Persistence.DbContext;
using Vettingo.ApplicationService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.Enrich.FromLogContext().WriteTo.Console());

builder.Services.AddApplicationPersistence(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreateJobApplicationCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateJobApplicationCommandRequest>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await database.Database.EnsureCreatedAsync();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
