using Microsoft.EntityFrameworkCore;
using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

[Index(nameof(UserId),IsUnique =true)]
public sealed class Cv : BaseEntity
{
    private readonly List<Education> _educations = new();
    private readonly List<Project> _projects = new();
    private readonly List<Experience> _experiences = new();
    private readonly List<Certificate> _certificates = new();
    private readonly List<Language> _languages = new();
    private readonly List<CandidateSkill> _candidateSkills = new();

    public Guid UserId { get; private set; }
    public string TargetRole { get; private set; } = string.Empty;
    public int TargetSalary { get; private set; }
    public IReadOnlyCollection<Language> Languages => _languages.AsReadOnly();
    public IReadOnlyCollection<Education> Educations => _educations.AsReadOnly();
    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();
    public IReadOnlyCollection<Experience> Experiences => _experiences.AsReadOnly();
    public IReadOnlyCollection<Certificate> Certificates => _certificates.AsReadOnly();
    public IReadOnlyCollection<CandidateSkill> CandidateSkills => _candidateSkills.AsReadOnly();

    public void Create(Guid userId, string targetRole, int targetSalary, IReadOnlyCollection<Language>? languages)
    {
        Initialize();
        UserId = userId;
        TargetRole = targetRole;
        TargetSalary = targetSalary;
        if (languages is not null)
        {
            SetLanguages(languages);
        }
    }

    public void SetLanguages(IReadOnlyCollection<Language> languages)
    {
        _languages.Clear();
        _languages.AddRange(languages);
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
