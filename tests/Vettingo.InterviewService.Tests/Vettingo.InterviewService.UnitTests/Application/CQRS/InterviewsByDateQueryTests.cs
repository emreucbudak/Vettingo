using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;
using Vettingo.InterviewService.Application.Validations;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Application.CQRS;

public sealed class InterviewsByDateQueryTests
{
    [Fact]
    public async Task Should_Filter_By_Company_And_Selected_Date_And_Return_Cards_In_Time_Order()
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        var date = new DateOnly(2026, 10, 11);
        AddInterview(context, companyId, date, new TimeOnly(13, 30), "Ceren");
        var first = AddInterview(context, companyId, date, new TimeOnly(9, 30), "Ayşe");
        AddInterview(context, companyId, date.AddDays(-1), new TimeOnly(8, 0), "Dün");
        AddInterview(context, companyId, date.AddDays(1), new TimeOnly(8, 0), "Yarın");
        AddInterview(context, Guid.NewGuid(), date, new TimeOnly(8, 0), "Diğer şirket");
        await context.SaveChangesAsync();
        var handler = new GetInterviewsByDateQueryHandler(new Repository<Interview>(context));

        var result = (await handler.Handle(new(companyId, date), CancellationToken.None)).ToArray();

        result.Select(interview => interview.Name).Should().Equal("Ayşe", "Ceren");
        result.Select(interview => interview.StartedTime).Should().Equal(new TimeOnly(9, 30), new TimeOnly(13, 30));
        result[0].Id.Should().Be(first.Id);
        result[0].UserId.Should().Be(first.UserId);
        result[0].InterviewDate.Should().Be(date);
        result[0].Surname.Should().Be("Yılmaz");
        result[0].Role.Should().Be("Backend Geliştirici");
        result[0].Chapter.Should().Be("Teknik Mülakat");
        result[0].WhereIsMeeting.Should().Be("Microsoft Teams");
        result[0].MeetingLink.Should().Be("https://teams.microsoft.com/example");
    }

    [Fact]
    public async Task Should_Return_An_Empty_List_When_Selected_Date_Has_No_Interviews()
    {
        await using var context = CreateContext();
        var companyId = Guid.NewGuid();
        var date = new DateOnly(2026, 10, 11);
        AddInterview(context, companyId, date.AddDays(1), new TimeOnly(9, 0), "Başka gün");
        await context.SaveChangesAsync();
        var handler = new GetInterviewsByDateQueryHandler(new Repository<Interview>(context));

        var result = await handler.Handle(new(companyId, date), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Should_Require_Company_And_Date()
    {
        var validator = new GetInterviewsByDateQueryRequestValidator();
        var date = new DateOnly(2026, 10, 11);

        validator.Validate(new GetInterviewsByDateQueryRequest(Guid.Empty, date)).IsValid.Should().BeFalse();
        validator.Validate(new GetInterviewsByDateQueryRequest(Guid.NewGuid(), default)).IsValid.Should().BeFalse();
        validator.Validate(new GetInterviewsByDateQueryRequest(Guid.NewGuid(), date)).IsValid.Should().BeTrue();
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static Interview AddInterview(InterviewDbContext context, Guid companyId, DateOnly date, TimeOnly time, string name)
    {
        var interview = new Interview();
        var entry = context.Entry(interview);
        entry.Property(entity => entity.Id).CurrentValue = Guid.NewGuid();
        entry.Property(entity => entity.CompanyId).CurrentValue = companyId;
        entry.Property(entity => entity.UserId).CurrentValue = Guid.NewGuid();
        entry.Property(entity => entity.InterviewDate).CurrentValue = date;
        entry.Property(entity => entity.StartedTime).CurrentValue = time;
        entry.Property(entity => entity.Name).CurrentValue = name;
        entry.Property(entity => entity.Surname).CurrentValue = "Yılmaz";
        entry.Property(entity => entity.Chapter).CurrentValue = "Teknik Mülakat";
        entry.Property(entity => entity.Role).CurrentValue = "Backend Geliştirici";
        entry.Property(entity => entity.WhereIsMeeting).CurrentValue = "Microsoft Teams";
        entry.Property(entity => entity.MeetingLink).CurrentValue = "https://teams.microsoft.com/example";
        context.Interviews.Add(interview);
        return interview;
    }
}
