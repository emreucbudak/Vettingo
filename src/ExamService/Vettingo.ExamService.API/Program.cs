using FlashMediator;
using FluentValidation;
using Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam;
using Vettingo.ExamService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddFlashMediator(typeof(CreateExamCommandHandler).Assembly);
builder.Services.AddValidatorsFromAssemblyContaining<CreateExamCommandRequest>();

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
