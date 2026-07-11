using EvaluationEntity = Vettingo.EvaluationService.Domain.Models.Evaluation;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation;

internal static class EvaluationMapper
{
    internal static EvaluationResponse ToResponse(this EvaluationEntity evaluation)
    {
        return new EvaluationResponse
        {
            Id = evaluation.Id,
            UserId = evaluation.UserId,
            SkillName = evaluation.SkillName,
            SkillLevel = evaluation.SkillLevel,
            OverallScore = evaluation.OverallScore,
            CreatedAt = evaluation.CreatedAt,
            UpdatedAt = evaluation.UpdatedAt
        };
    }
}
