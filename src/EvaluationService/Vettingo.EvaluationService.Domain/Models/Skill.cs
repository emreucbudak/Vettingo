using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

[Index(nameof(Title), IsUnique = true)]
public sealed class Skill : BaseEntity
{
    public string Title { get; set; } = string.Empty;
}
