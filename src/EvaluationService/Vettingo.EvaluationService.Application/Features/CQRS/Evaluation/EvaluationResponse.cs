namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation;

public sealed record EvaluationResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string SkillName { get; init; } = string.Empty;
    public int SkillLevel { get; init; }
    public int OverallScore { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
