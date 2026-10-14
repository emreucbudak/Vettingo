using FlashMediator;

namespace Vettingo.EvaluationService.Application.Features.CQRS.Scout.Query.GetScoutByFilter;

public sealed class GetScoutByFilterQueryRequest : IRequest<GetScoutByFilterQueryResponse>
{
    public string Role { get; init; } = string.Empty;
    public int? MinSalary { get; init; }
    public int? MaxSalary { get; init; }
    public string? WorkPreference { get; init; }
    public int? MinExperienceYear { get; init; }
    public int? MaxExperienceYear { get; init; }
    public string? Location { get; init; }
}
