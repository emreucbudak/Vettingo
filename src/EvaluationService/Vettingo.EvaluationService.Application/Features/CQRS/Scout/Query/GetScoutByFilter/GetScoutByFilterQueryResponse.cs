namespace Vettingo.EvaluationService.Application.Features.CQRS.Scout.Query.GetScoutByFilter;

public sealed record GetScoutByFilterQueryResponse
{
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public int Salary { get; init; }
    public int Rating { get; init; }
    public string Role { get; init; } = string.Empty;
    public string WorkPreference { get; init; } = string.Empty;
    public IList<string> Skills { get; init; }
}
