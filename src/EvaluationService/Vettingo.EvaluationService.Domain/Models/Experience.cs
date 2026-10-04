using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Experience : BaseEntity
{
    public string CompanyName { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string City { get; private set; } = string.Empty;
}
