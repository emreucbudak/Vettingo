using FlashMediator;
using Vettingo.EvaluationService.Application.Interfaces;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Evaluation.Query.GetEvaluationById;

public sealed record GetEvaluationByIdQueryRequest : IRequest<EvaluationResponse>, ICacheableQuery
{
    private string? _cacheKey;

    public Guid EvaluationId { get; init; }

    public string CacheKey
    {
        get => _cacheKey ?? $"Evaluation:GetById:{EvaluationId}";
        set => _cacheKey = value;
    }

    public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromMinutes(10);
}
