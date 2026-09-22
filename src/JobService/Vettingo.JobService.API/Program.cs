using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Persistence.DbContext;
using System.Threading.RateLimiting;
using Serilog;
using FlashMediator;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Vettingo.JobService.API.ExceptionHandlers;
using Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;
using Vettingo.JobService.Persistence.Registration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

string jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");
string jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
    ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured.");
string jwtAudience = builder.Configuration["JwtSettings:Audience"]
    ?? throw new InvalidOperationException("JwtSettings:Audience is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "Role",
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: context.User.FindFirst(ClaimTypes.Email)!.Value.Trim().ToLowerInvariant(),
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

builder.Services.SaveDb(builder.Configuration);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")
        ?? throw new InvalidOperationException("Connection string 'Redis' is not configured.");
    options.InstanceName = "Vettingo:JobService:";
});
builder.Services.AddFlashMediator(typeof(CreateJobPostingCommandHandler).Assembly);
builder.Services.AddFlashMediatorHybridCache();
builder.Services.AddValidatorsFromAssemblyContaining<CreateJobPostingCommandRequest>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();
builder.Services.AddExceptionHandler<BaseExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter<Vettingo.JobService.Domain.Enums.ApplicationStatus>()));

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var database = scope.ServiceProvider.GetRequiredService<JobDbContext>();
    await database.Database.EnsureCreatedAsync();
    await database.Database.ExecuteSqlRawAsync("""
        CREATE TABLE IF NOT EXISTS "JobApplications" (
            "Id" uuid PRIMARY KEY,
            "CandidateId" uuid NOT NULL,
            "JobPostingId" uuid NOT NULL REFERENCES "JobPostings" ("Id") ON DELETE RESTRICT,
            "AppliedAt" timestamp with time zone NOT NULL,
            "Status" text NOT NULL,
            "CreatedAt" timestamp with time zone NOT NULL,
            "UpdatedAt" timestamp with time zone NULL
        );
        CREATE INDEX IF NOT EXISTS "IX_JobApplications_CandidateId" ON "JobApplications" ("CandidateId");
        CREATE INDEX IF NOT EXISTS "IX_JobApplications_JobPostingId" ON "JobApplications" ("JobPostingId");
        CREATE INDEX IF NOT EXISTS "IX_JobApplications_AppliedAt" ON "JobApplications" ("AppliedAt");
        """);
}

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} isteği {StatusCode} durum koduyla {Elapsed:0.0000} ms içinde tamamlandı";
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();


app.MapControllers();

app.Run();

