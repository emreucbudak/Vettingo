using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetToday;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS
{
    public class TodayInterviewsQueryTests
    {
        [Fact]
        public async Task Should_Return_Only_Todays_Company_Interviews_In_Time_Order()
        {
            await using var context = CreateContext();
            var companyId = Guid.NewGuid();
            var today = new DateOnly(2026, 10, 8);
            AddInterview(context, companyId, today, new TimeOnly(13, 30), "Ceren", "Aksoy");
            AddInterview(context, companyId, today, new TimeOnly(9, 30), "Ayşe", "Yılmaz");
            AddInterview(context, companyId, today.AddDays(-1), new TimeOnly(8, 0), "Dün", "Aday");
            AddInterview(context, companyId, today.AddDays(1), new TimeOnly(8, 0), "Yarın", "Aday");
            AddInterview(context, Guid.NewGuid(), today, new TimeOnly(8, 0), "Diğer", "Şirket");
            await context.SaveChangesAsync();
            var handler = new GetTodayInterviewsQueryHandler(new Repository<Interview>(context),
                new FixedTimeProvider(DateTimeOffset.Parse("2026-10-08T09:00:00Z")));

            var result = (await handler.Handle(new(companyId), CancellationToken.None)).ToArray();

            result.Should().HaveCount(2);
            result.Select(interview => interview.StartedTime).Should().Equal(new TimeOnly(9, 30), new TimeOnly(13, 30));
            result[0].Name.Should().Be("Ayşe");
            result[0].Surname.Should().Be("Yılmaz");
            result[0].Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("2026-10-07T20:59:59Z", 7)]
        [InlineData("2026-10-07T21:00:00Z", 8)]
        public async Task Should_Use_Istanbul_Date_At_Midnight(string utcNow, int expectedDay)
        {
            await using var context = CreateContext();
            var companyId = Guid.NewGuid();
            AddInterview(context, companyId, new DateOnly(2026, 10, 7), new TimeOnly(9, 0), "7", "Ekim");
            AddInterview(context, companyId, new DateOnly(2026, 10, 8), new TimeOnly(9, 0), "8", "Ekim");
            await context.SaveChangesAsync();
            var handler = new GetTodayInterviewsQueryHandler(new Repository<Interview>(context),
                new FixedTimeProvider(DateTimeOffset.Parse(utcNow)));

            var result = await handler.Handle(new(companyId), CancellationToken.None);

            result.Should().ContainSingle().Which.Name.Should().Be(expectedDay.ToString());
        }

        [Fact]
        public async Task Should_Return_Empty_When_No_Interview_Is_Scheduled_Today()
        {
            await using var context = CreateContext();
            var handler = new GetTodayInterviewsQueryHandler(new Repository<Interview>(context),
                new FixedTimeProvider(DateTimeOffset.Parse("2026-10-08T09:00:00Z")));

            var result = await handler.Handle(new(Guid.NewGuid()), CancellationToken.None);

            result.Should().BeEmpty();
        }

        [Fact]
        public void PostgreSql_Should_Map_Date_And_Time_And_Translate_The_Filter()
        {
            var options = new DbContextOptionsBuilder<InterviewDbContext>()
                .UseNpgsql("Host=localhost;Database=InterviewQueryTest")
                .Options;
            using var context = new InterviewDbContext(options);
            var companyId = Guid.NewGuid();
            var today = new DateOnly(2026, 10, 8);
            var entityType = context.Model.FindEntityType(typeof(Interview))!;

            entityType.FindProperty(nameof(Interview.InterviewDate))!.GetColumnType().Should().Be("date");
            entityType.FindProperty(nameof(Interview.StartedTime))!.GetColumnType().Should().Be("time without time zone");
            var sql = context.Interviews
                .Where(interview => interview.CompanyId == companyId && interview.InterviewDate == today)
                .OrderBy(interview => interview.StartedTime)
                .ToQueryString();
            sql.Should().Contain("WHERE").And.Contain("\"CompanyId\"").And.Contain("\"InterviewDate\"")
                .And.Contain("ORDER BY").And.Contain("\"StartedTime\"");
        }

        private static InterviewDbContext CreateContext() => new(
            new DbContextOptionsBuilder<InterviewDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        private static void AddInterview(InterviewDbContext context, Guid companyId,
            DateOnly date, TimeOnly time, string name, string surname)
        {
            var interview = new Interview();
            var entry = context.Entry(interview);
            entry.Property(entity => entity.Id).CurrentValue = Guid.NewGuid();
            entry.Property(entity => entity.CompanyId).CurrentValue = companyId;
            entry.Property(entity => entity.UserId).CurrentValue = Guid.NewGuid();
            entry.Property(entity => entity.InterviewDate).CurrentValue = date;
            entry.Property(entity => entity.StartedTime).CurrentValue = time;
            entry.Property(entity => entity.Name).CurrentValue = name;
            entry.Property(entity => entity.Surname).CurrentValue = surname;
            context.Interviews.Add(interview);
        }

        private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() => utcNow;
        }
    }
}
