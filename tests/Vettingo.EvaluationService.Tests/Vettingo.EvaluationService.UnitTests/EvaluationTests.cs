using FluentAssertions;
using Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Command.CreateEvaluation;
using Vettingo.EvaluationService.Application.Validations;
using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.UnitTests;

public sealed class EvaluationTests
{
    [Fact]
    public void Create_WithValidValues_ShouldInitializeEvaluation()
    {
        Guid userId = Guid.NewGuid();
        EvaluationEntity evaluation = new();

        evaluation.Create(userId, "  C#  ", 85, 90);

        evaluation.Id.Should().NotBeEmpty();
        evaluation.UserId.Should().Be(userId);
        evaluation.SkillName.Should().Be("C#");
        evaluation.SkillLevel.Should().Be(85);
        evaluation.OverallScore.Should().Be(90);
        evaluation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        evaluation.UpdatedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_WithInvalidSkillLevel_ShouldThrow(int skillLevel)
    {
        EvaluationEntity evaluation = new();

        Action action = () => evaluation.Create(Guid.NewGuid(), "C#", skillLevel, 90);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_WithInvalidOverallScore_ShouldThrow(int overallScore)
    {
        EvaluationEntity evaluation = new();

        Action action = () => evaluation.Create(Guid.NewGuid(), "C#", 85, overallScore);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CreateValidator_WithOutOfRangeScores_ShouldBeInvalid()
    {
        CreateEvaluationCommandRequest request = new()
        {
            UserId = Guid.NewGuid(),
            SkillName = "C#",
            SkillLevel = 101,
            OverallScore = -1
        };

        CreateEvaluationCommandRequestValidator validator = new();
        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.PropertyName == nameof(request.SkillLevel));
        result.Errors.Should().Contain(error => error.PropertyName == nameof(request.OverallScore));
    }
}
