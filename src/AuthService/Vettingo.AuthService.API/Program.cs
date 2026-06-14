using FluentValidation;
using Vettingo.AuthService.Infrastructure.Register;
using Vettingo.AuthService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.SaveDb(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTokenSettings(builder.Configuration);

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
