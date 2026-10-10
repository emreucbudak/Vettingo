using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetCompanyStatistics;
using Vettingo.InterviewService.Application.Validations;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class CompanyInterviewStatisticsQueryTests
{
    [Fact]
    public async Task Should_Count_Company_Interviews_In_Full_Calendar_Periods()
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        AddInterviews(context, companyId,
            new(2025, 10, 8), new(2026, 9, 30), new(2026, 10, 1), new(2026, 10, 4),
            new(2026, 10, 5), new(2026, 10, 8), new(2026, 10, 8), new(2026, 10, 11),
            new(2026, 10, 12), new(2026, 10, 31), new(2026, 11, 1));
        AddInterviews(context, Guid.NewGuid(), new(2026, 10, 8), new(2026, 10, 11), new(2026, 10, 31));
        await context.SaveChangesAsync();

        var result = await Handler(context, "2026-10-08T09:00:00Z")
            .Handle(new(companyId), CancellationToken.None);

        result.Should().Be(new GetCompanyInterviewStatisticsQueryResponse(11, 8, 4, 2));
    }

    [Fact]
    public async Task Should_Count_Weeks_That_Cross_The_Year_And_Exclude_Next_Monday()
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        AddInterviews(context, companyId,
            new(2026, 12, 27), new(2026, 12, 28), new(2026, 12, 31), new(2027, 1, 1),
            new(2027, 1, 3), new(2027, 1, 4), new(2027, 1, 31), new(2027, 2, 1));
        await context.SaveChangesAsync();

        var result = await Handler(context, "2027-01-01T09:00:00Z")
            .Handle(new(companyId), CancellationToken.None);

        result.Should().Be(new GetCompanyInterviewStatisticsQueryResponse(8, 4, 4, 1));
    }

    [Theory]
    [InlineData("2026-10-11T20:59:59Z", 3, 2)]
    [InlineData("2026-10-11T21:00:00Z", 2, 1)]
    public async Task Should_Change_Today_And_Week_At_Istanbul_Monday_Midnight(
        string utcNow, int expectedWeek, int expectedToday)
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        AddInterviews(context, companyId,
            new(2026, 10, 5), new(2026, 10, 11), new(2026, 10, 11),
            new(2026, 10, 12), new(2026, 10, 18), new(2026, 10, 19));
        await context.SaveChangesAsync();

        var result = await Handler(context, utcNow).Handle(new(companyId), CancellationToken.None);

        result.Should().Be(new GetCompanyInterviewStatisticsQueryResponse(6, 6, expectedWeek, expectedToday));
    }

    [Theory]
    [InlineData("2026-09-30T20:59:59Z", 2, 2)]
    [InlineData("2026-09-30T21:00:00Z", 3, 1)]
    public async Task Should_Change_Today_And_Month_At_Istanbul_Month_Midnight(
        string utcNow, int expectedMonth, int expectedToday)
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        AddInterviews(context, companyId,
            new(2026, 9, 30), new(2026, 9, 30), new(2026, 10, 1), new(2026, 10, 31), new(2026, 10, 31));
        await context.SaveChangesAsync();

        var result = await Handler(context, utcNow).Handle(new(companyId), CancellationToken.None);

        result.Should().Be(new GetCompanyInterviewStatisticsQueryResponse(5, expectedMonth, 3, expectedToday));
    }

    [Fact]
    public async Task Should_Return_Zero_When_Company_Has_No_Interviews()
    {
        await using var context = CreateContext();
        AddInterviews(context, Guid.NewGuid(), new DateOnly(2026, 10, 8));
        await context.SaveChangesAsync();

        var result = await Handler(context, "2026-10-08T09:00:00Z")
            .Handle(new(Guid.NewGuid()), CancellationToken.None);

        result.Should().Be(new GetCompanyInterviewStatisticsQueryResponse(0, 0, 0, 0));
    }

    [Fact]
    public async Task Should_Honor_Cancellation()
    {
        await using var context = CreateContext();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        var act = async () => await Handler(context, "2026-10-08T09:00:00Z")
            .Handle(new(Guid.NewGuid()), cancellation.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task PostgreSql_Should_Translate_Statistics_To_One_Company_Filtered_Aggregate()
    {
        var queryPlans = new List<string>();
        var options = new DbContextOptionsBuilder<InterviewDbContext>()
            .UseNpgsql("Host=localhost;Database=InterviewStatisticsTranslationTest")
            .LogTo(queryPlans.Add, [CoreEventId.QueryExecutionPlanned])
            .AddInterceptors(new StopBeforeDatabaseConnectionInterceptor())
            .Options;
        await using var context = new InterviewDbContext(options);

        var act = async () => await new InterviewRepository(context).GetCompanyStatisticsAsync(
            Guid.NewGuid(), new DateOnly(2026, 10, 8), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("SQL translation checked; database connection skipped.");
        queryPlans.Should().ContainSingle().Which.Should().Contain("count(*)")
            .And.Contain("WHERE i.CompanyId == @companyId").And.Contain("InterviewDate")
            .And.Contain("FILTER").And.Contain("SingleQueryingEnumerable");
    }

    [Fact]
    public void Should_Require_A_Company_Id()
    {
        var validator = new GetCompanyInterviewStatisticsQueryRequestValidator();

        validator.Validate(new GetCompanyInterviewStatisticsQueryRequest(Guid.Empty)).IsValid.Should().BeFalse();
        validator.Validate(new GetCompanyInterviewStatisticsQueryRequest(Guid.NewGuid())).IsValid.Should().BeTrue();
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static GetCompanyInterviewStatisticsQueryHandler Handler(InterviewDbContext context, string utcNow) =>
        new(new InterviewRepository(context), new FixedTimeProvider(DateTimeOffset.Parse(utcNow)));

    private static void AddInterviews(InterviewDbContext context, Guid companyId, params DateOnly[] dates)
    {
        foreach (var date in dates)
        {
            var interview = new Interview();
            var entry = context.Entry(interview);
            entry.Property(entity => entity.CompanyId).CurrentValue = companyId;
            entry.Property(entity => entity.UserId).CurrentValue = Guid.NewGuid();
            entry.Property(entity => entity.InterviewDate).CurrentValue = date;
            entry.Property(entity => entity.StartedTime).CurrentValue = new TimeOnly(9, 0);
            context.Interviews.Add(interview);
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }

    private sealed class StopBeforeDatabaseConnectionInterceptor : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = default) =>
            throw new InvalidOperationException("SQL translation checked; database connection skipped.");
    }
}
