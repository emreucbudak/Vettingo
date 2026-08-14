var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origins)
        .AllowAnyHeader()
        .AllowCredentials() 
        .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
