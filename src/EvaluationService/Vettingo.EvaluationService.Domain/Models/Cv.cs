using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

[Index(nameof(UserId),IsUnique =true)]
public sealed class Cv : BaseEntity
{
    private readonly List<Education> _educations = new();
    private readonly List<Project> _projects = new();
    private readonly List<Experience> _experiences = new();

    public Guid UserId { get; private set; }
    public string TargetRole { get; private set; } = string.Empty;
    public int TargetSalary { get; private set; }
    public Dictionary<string, int> Skills { get; private set; } = new();
    public Dictionary<string, string> Languages { get; private set; } = new();
    public IReadOnlyCollection<Education> Educations => _educations.AsReadOnly();
    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();
    public IReadOnlyCollection<Experience> Experiences => _experiences.AsReadOnly();

    public void Create(Guid userId, string targetRole, int targetSalary,
        Dictionary<string, int>? skills, Dictionary<string, string>? languages)
    {
        Initialize();
        UserId = userId;
        TargetRole = targetRole;
        TargetSalary = targetSalary;
        if (skills is not null)
        {
            Skills = new Dictionary<string, int>(skills);
        }
        if (languages is not null)
        {
            Languages = new Dictionary<string, string>(languages);
        }
    }

    public void AddEducation(Education education)
    {
        ArgumentNullException.ThrowIfNull(education);
        _educations.Add(education);
    }

    public void AddProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        _projects.Add(project);
    }

    public void AddExperience(Experience experience)
    {
        ArgumentNullException.ThrowIfNull(experience);
        _experiences.Add(experience);
    }
}
