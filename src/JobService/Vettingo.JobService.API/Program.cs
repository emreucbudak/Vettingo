using FlashMediator;
using FluentValidation;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;
using Vettingo.JobService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreateJobPostingCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateJobPostingCommandRequest>();

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

app.Run();
