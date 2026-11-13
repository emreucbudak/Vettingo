using Microsoft.EntityFrameworkCore;
using Vettingo.JobService.Domain.Common;

namespace Vettingo.JobService.Domain.Entities;

[Index(nameof(Title), IsUnique = true)]
public sealed class Skill : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public void CreateSkill(string title)
    {
        SetId();
        Title = title;
        SetCreatedAt();
    }
}
