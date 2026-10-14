namespace Vettingo.EvaluationService.Application.Features.CQRS.Scout.Query.GetScoutByFilter;

public sealed record GetScoutByFilterQueryResponse
{
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public int Salary { get; init; }
}
