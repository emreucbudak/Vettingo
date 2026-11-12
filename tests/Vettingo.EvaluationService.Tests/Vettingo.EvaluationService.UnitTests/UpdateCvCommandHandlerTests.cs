using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Update;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Domain.Enums;
using Vettingo.EvaluationService.Domain.Models;
using Vettingo.EvaluationService.Persistence.DbContext;
using Vettingo.EvaluationService.Persistence.Repository;

namespace Vettingo.EvaluationService.UnitTests;

public sealed class UpdateCvCommandHandlerTests
{
    [Fact]
    public async Task Handler_Should_Forward_Optional_Values_And_Save_With_Cancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var cv = CreateCv();
        var request = new UpdateCvCommandRequest
        {
            UserId = cv.UserId,
            TargetSalary = 0,
            Educations = []
        };
        var repository = Substitute.For<ICvRepository>();
        repository.GetByUserIdAsync(request.UserId, cancellation.Token).Returns(cv);

        await new UpdateCvCommandHandler(repository).Handle(request, cancellation.Token);

        await repository.Received(1).GetByUserIdAsync(request.UserId, cancellation.Token);
        repository.Received(1).Update(cv, null, 0, null, request.Educations, null, null);
        await repository.Received(1).SaveChangesAsync(cancellation.Token);
    }

    [Fact]
    public async Task Handler_Should_Throw_And_Not_Save_When_Cv_Does_Not_Exist()
    {
        var repository = Substitute.For<ICvRepository>();
        var request = new UpdateCvCommandRequest { UserId = Guid.NewGuid() };

        var action = () => new UpdateCvCommandHandler(repository).Handle(request, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>();
        await repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Repository_Should_Replace_Supplied_Values_And_Track_Old_Children_For_Deletion()
    {
        using var context = CreateContext();
        var cv = AttachExistingCv(context);
        var id = cv.Id;
        var createdAt = cv.CreatedAt;
        var oldLanguage = cv.Languages.Single();
        var language = new Language("Turkish", LanguageLevel.C2);
        var oldEducation = cv.Educations.Single();
        var oldProject = cv.Projects.Single();
        var oldExperience = cv.Experiences.Single();
        var education = new Education();
        var project = new Project();
        var experience = new Experience();

        new CvRepository(context).Update(cv, "Developer", 0,
            [language],
            [education], [project], [experience]);
        context.ChangeTracker.DetectChanges();

        cv.TargetRole.Should().Be("Developer");
        cv.TargetSalary.Should().Be(0);
        cv.Languages.Should().ContainSingle().Which.Should().BeSameAs(language);
        context.Entry(oldLanguage).State.Should().Be(EntityState.Deleted);
        context.Entry(language).State.Should().Be(EntityState.Added);
        cv.Educations.Should().ContainSingle().Which.Should().BeSameAs(education);
        cv.Projects.Should().ContainSingle().Which.Should().BeSameAs(project);
        cv.Experiences.Should().ContainSingle().Which.Should().BeSameAs(experience);
        cv.Id.Should().Be(id);
        cv.CreatedAt.Should().Be(createdAt);
        cv.UpdatedAt.Should().NotBeNull();
        context.Entry(oldEducation).State.Should().Be(EntityState.Deleted);
        context.Entry(oldProject).State.Should().Be(EntityState.Deleted);
        context.Entry(oldExperience).State.Should().Be(EntityState.Deleted);
        context.Entry(education).State.Should().Be(EntityState.Added);
        context.Entry(project).State.Should().Be(EntityState.Added);
        context.Entry(experience).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public void Repository_Should_Keep_All_Values_When_Update_Fields_Are_Null()
    {
        using var context = CreateContext();
        var cv = AttachExistingCv(context);
        var education = cv.Educations.Single();
        var project = cv.Projects.Single();
        var experience = cv.Experiences.Single();

        new CvRepository(context).Update(cv, null, null, null, null, null, null);
        context.ChangeTracker.DetectChanges();

        cv.TargetRole.Should().Be("Backend Developer");
        cv.TargetSalary.Should().Be(75000);
        cv.Languages.Should().ContainSingle().Which.LanguageName.Should().Be("English");
        cv.Educations.Should().ContainSingle().Which.Should().BeSameAs(education);
        cv.Projects.Should().ContainSingle().Which.Should().BeSameAs(project);
        cv.Experiences.Should().ContainSingle().Which.Should().BeSameAs(experience);
        cv.UpdatedAt.Should().BeNull();
        context.ChangeTracker.HasChanges().Should().BeFalse();
    }

    [Fact]
    public void Repository_Should_Clear_Only_Supplied_Empty_Collections()
    {
        using var context = CreateContext();
        var cv = AttachExistingCv(context);

        new CvRepository(context).Update(cv, null, null, [], [], [], []);
        context.ChangeTracker.DetectChanges();

        cv.TargetRole.Should().Be("Backend Developer");
        cv.TargetSalary.Should().Be(75000);
        cv.Languages.Should().BeEmpty();
        cv.Educations.Should().BeEmpty();
        cv.Projects.Should().BeEmpty();
        cv.Experiences.Should().BeEmpty();
    }

    private static Cv CreateCv()
    {
        var cv = new Cv();
        cv.Create(Guid.NewGuid(), "Backend Developer", 75000,
            [new("English", LanguageLevel.B2)]);
        cv.AddEducation(new Education());
        cv.AddProject(new Project());
        cv.AddExperience(new Experience());
        return cv;
    }

    private static Cv AttachExistingCv(EvaluationDbContext context)
    {
        var cv = CreateCv();
        context.Cvs.Attach(cv);
        foreach (var entry in context.ChangeTracker.Entries().ToArray())
        {
            entry.State = EntityState.Unchanged;
        }
        context.ChangeTracker.Clear();
        return cv;
    }

    private static EvaluationDbContext CreateContext() => new(
        new DbContextOptionsBuilder<EvaluationDbContext>()
            .UseNpgsql("Host=localhost;Database=unused;Username=unused").Options);
}
