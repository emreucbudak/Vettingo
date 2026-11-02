using FluentAssertions;
using NSubstitute;
using Vettingo.EvaluationService.Application.Exceptions;
using Vettingo.EvaluationService.Application.Features.CQRS.Cv.Query.GetCvByUserIdQueries;
using Vettingo.EvaluationService.Application.Repository;
using Vettingo.EvaluationService.Domain.Models;

namespace Vettingo.EvaluationService.UnitTests;

public sealed class GetCvByUserIdQueriesTests
{
    [Fact]
    public async Task Handler_Should_Return_Cv_Fields_And_Forward_UserId_And_Cancellation()
    {
        var userId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var cv = new Cv();
        typeof(Cv).GetProperty(nameof(Cv.UserId))!.SetValue(cv, userId);
        typeof(Cv).GetProperty(nameof(Cv.TargetRole))!.SetValue(cv, "Backend Developer");
        typeof(Cv).GetProperty(nameof(Cv.TargetSalary))!.SetValue(cv, 75000);
        cv.Skills.Add("C#", 4);
        cv.Languages.Add("English", "B2");
        cv.AddEducation(new Education());
        cv.AddProject(new Project());
        cv.AddExperience(new Experience());
        var repository = Substitute.For<ICvRepository>();
        repository.GetByUserIdAsync(userId, cancellation.Token).Returns(cv);

        var response = await new GetCvByUserIdQueriesHandler(repository).Handle(
            new GetCvByUserIdQueriesRequest { UserId = userId }, cancellation.Token);

        response.TargetRole.Should().Be("Backend Developer");
        response.TargetSalary.Should().Be(75000);
        response.Skills.Should().BeEquivalentTo(cv.Skills);
        response.Languages.Should().BeEquivalentTo(cv.Languages);
        response.Educations.Should().ContainSingle().Which.Should().BeSameAs(cv.Educations.Single());
        response.Projects.Should().ContainSingle().Which.Should().BeSameAs(cv.Projects.Single());
        response.Experiences.Should().ContainSingle().Which.Should().BeSameAs(cv.Experiences.Single());
        await repository.Received(1).GetByUserIdAsync(userId, cancellation.Token);
    }

    [Fact]
    public async Task Handler_Should_Throw_NotFound_When_User_Has_No_Cv()
    {
        var userId = Guid.NewGuid();
        var repository = Substitute.For<ICvRepository>();
        repository.GetByUserIdAsync(userId, CancellationToken.None).Returns((Cv?)null);

        var action = () => new GetCvByUserIdQueriesHandler(repository).Handle(
            new GetCvByUserIdQueriesRequest { UserId = userId }, CancellationToken.None);

        await action.Should().ThrowAsync<NotFoundException>().WithMessage("CV bulunamadı.");
    }
}
