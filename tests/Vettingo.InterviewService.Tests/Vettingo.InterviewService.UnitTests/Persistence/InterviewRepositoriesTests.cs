using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Vettingo.InterviewService.Domain.Entities;
using Vettingo.InterviewService.Persistence.DbContext;
using Vettingo.InterviewService.Persistence.Repository;

namespace Vettingo.InterviewService.UnitTests.Persistence;

public sealed class InterviewRepositoriesTests
{
    [Fact]
    public async Task Interview_Repository_Should_Save_Update_And_Delete_Detached_Entities()
    {
        await using var context = CreateContext();
        var repository = new InterviewRepository(context);
        var interview = new Interview();
        var companyId = Guid.NewGuid();
        var date = new DateOnly(2026, 10, 30);
        var jobInterview = new JobInterview();
        jobInterview.CreateJobInterview(Guid.NewGuid(), date, date, new TimeOnly(9, 0), new TimeOnly(17, 0), []);
        context.Add(jobInterview);
        await context.SaveChangesAsync();
        context.Entry(interview).Property(entity => entity.JobInterviewId).CurrentValue = jobInterview.Id;
        context.Entry(interview).Property(entity => entity.CompanyId).CurrentValue = companyId;
        context.Entry(interview).Property(entity => entity.InterviewDate).CurrentValue = date;
        await repository.AddAsync(interview);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var saved = (await repository.GetByIdAsync(interview.Id))!;
        saved.CompanyId.Should().Be(companyId);
        context.ChangeTracker.Clear();
        context.Entry(saved).Property(entity => entity.Role).CurrentValue = "Backend Geliştirici";
        repository.Update(saved);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var results = await repository.GetByCompanyAndDateAsync(companyId, date);
        results.Should().ContainSingle().Which.Role.Should().Be("Backend Geliştirici");
        (await repository.GetByCompanyAndDateAsync(Guid.NewGuid(), date)).Should().BeEmpty();

        context.ChangeTracker.Clear();
        repository.Delete(saved);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();
        (await repository.GetByIdAsync(interview.Id)).Should().BeNull();
    }

    [Fact]
    public async Task Result_Repository_Should_Filter_By_Company_And_User_And_Update_And_Delete_Saved_Results()
    {
        await using var context = CreateContext();
        var repository = new InterviewResultRepository(context);
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var interviewId = AddInterview(context, companyId, userId);
        var sameCompanyInterviewId = AddInterview(context, companyId, Guid.NewGuid());
        var otherCompanyInterviewId = AddInterview(context, Guid.NewGuid(), Guid.NewGuid());
        var result = CreateResult(interviewId, 40);
        await repository.AddAsync(result);
        await repository.AddAsync(CreateResult(sameCompanyInterviewId, 90));
        await repository.AddAsync(CreateResult(otherCompanyInterviewId, 100));
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var saved = (await repository.GetByIdAsync(result.Id))!;
        context.ChangeTracker.Clear();
        context.Entry(saved).Property(entity => entity.InterviewScore).CurrentValue = 60;
        repository.Update(saved);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var results = await repository.GetAllByCompanyIdAsync(companyId);
        results.Select(entity => entity.InterviewScore).Should().BeEquivalentTo(new[] { 90, 60 });
        (await repository.GetByUserIdAsync(userId)).Should().ContainSingle().Which.Id.Should().Be(result.Id);
        (await repository.GetByUserIdAsync(Guid.NewGuid())).Should().BeEmpty();
        (await repository.GetAllByCompanyIdAsync(Guid.NewGuid())).Should().BeEmpty();

        context.ChangeTracker.Clear();
        repository.Delete(saved);
        await repository.SaveChangesAsync();
        context.ChangeTracker.Clear();
        (await repository.GetByIdAsync(result.Id)).Should().BeNull();
        (await repository.GetByUserIdAsync(userId)).Should().BeEmpty();
        (await repository.GetAllByCompanyIdAsync(companyId)).Should().ContainSingle()
            .Which.InterviewScore.Should().Be(90);
    }

    private static Guid AddInterview(InterviewDbContext context, Guid companyId, Guid userId)
    {
        var interview = new Interview();
        context.Entry(interview).Property(entity => entity.CompanyId).CurrentValue = companyId;
        context.Entry(interview).Property(entity => entity.UserId).CurrentValue = userId;
        context.Interviews.Add(interview);
        return interview.Id;
    }

    private static InterviewResult CreateResult(Guid interviewId, int score)
    {
        var result = new InterviewResult();
        result.Create(interviewId, score, score >= 50);
        return result;
    }

    private static InterviewDbContext CreateContext() => new(
        new DbContextOptionsBuilder<InterviewDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
