using FlashMediator;
using Vettingo.EvaluationService.Application.Interfaces;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluations;

public sealed record GetEvaluationsQueryRequest : IRequest<IReadOnlyList<EvaluationResponse>>, ICacheableQuery
{
    private string? _cacheKey;

    public Guid? UserId { get; init; }
    public string? SkillName { get; init; }

    public string CacheKey
    {
        get => _cacheKey ?? BuildCacheKey();
        set => _cacheKey = value;
    }

    public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromMinutes(10);

    private string BuildCacheKey()
    {
        string userId = UserId?.ToString() ?? "all";
        string skillName = string.IsNullOrWhiteSpace(SkillName) ? "all" : SkillName.Trim();
        return $"Evaluation:GetAll:UserId={userId}:SkillName={skillName}";
    }
}
