using FluentAssertions;
using NSubstitute;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Command.Create;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Domain.Enums;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.UnitTests;

public sealed class CreateCvCommandHandlerTests
{
    [Fact]
    public async Task Handler_Should_Create_Cv_With_Request_Lists_And_Save()
    {
        using var cancellation = new CancellationTokenSource();
        var education = new Education();
        var project = new Project();
        var experience = new Experience();
        var request = new CreateCvCommandRequest
        {
            UserId = Guid.NewGuid(),
            TargetRole = "Backend Developer",
            TargetSalary = 75000,
            Skills = new() { ["C#"] = 4 },
            Languages = [new("English", LanguageLevel.B2)],
            Educations = [education],
            Projects = [project],
            Experiences = [experience]
        };
        Cv? savedCv = null;
        var repository = Substitute.For<ICvRepository>();
        repository.AddAsync(Arg.Do<Cv>(cv => savedCv = cv), cancellation.Token)
            .Returns(Task.CompletedTask);
        repository.SaveChangesAsync(cancellation.Token).Returns(1);

        await new CreateCvCommandHandler(repository).Handle(request, cancellation.Token);

        savedCv.Should().NotBeNull();
        savedCv!.Id.Should().NotBeEmpty();
        savedCv.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        savedCv.UserId.Should().Be(request.UserId);
        savedCv.TargetRole.Should().Be(request.TargetRole);
        savedCv.TargetSalary.Should().Be(request.TargetSalary);
        savedCv.Skills.Should().BeEquivalentTo(request.Skills);
        var language = savedCv.Languages.Should().ContainSingle().Which;
        language.LanguageName.Should().Be("English");
        language.LanguageLevel.Should().Be(LanguageLevel.B2);
        savedCv.Educations.Should().ContainSingle().Which.Should().BeSameAs(education);
        savedCv.Projects.Should().ContainSingle().Which.Should().BeSameAs(project);
        savedCv.Experiences.Should().ContainSingle().Which.Should().BeSameAs(experience);
        await repository.Received(1).AddAsync(savedCv, cancellation.Token);
        await repository.Received(1).SaveChangesAsync(cancellation.Token);
    }

    [Fact]
    public async Task Handler_Should_Save_When_Optional_Request_Fields_Are_Null()
    {
        var request = new CreateCvCommandRequest
        {
            UserId = Guid.NewGuid(),
            TargetRole = "Backend Developer",
            TargetSalary = 75000
        };
        Cv? savedCv = null;
        var repository = Substitute.For<ICvRepository>();
        repository.AddAsync(Arg.Do<Cv>(cv => savedCv = cv), CancellationToken.None)
            .Returns(Task.CompletedTask);

        await new CreateCvCommandHandler(repository).Handle(request, CancellationToken.None);

        savedCv.Should().NotBeNull();
        savedCv!.Skills.Should().BeEmpty();
        savedCv.Languages.Should().BeEmpty();
        savedCv.Educations.Should().BeEmpty();
        savedCv.Projects.Should().BeEmpty();
        savedCv.Experiences.Should().BeEmpty();
        await repository.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
