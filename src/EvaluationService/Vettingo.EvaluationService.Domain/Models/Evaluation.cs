using Vettingo.EvaluationService.Domain.Common;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Evaluation : BaseEntity
{
    public Guid UserId { get; private set; }
    public string SkillName { get; private set; } = string.Empty;
    public int SkillLevel { get; private set; }
    public int OverallScore { get; private set; }

    public void Create(Guid userId, string skillName, int skillLevel, int overallScore)
    {
        Validate(userId, skillName, skillLevel, overallScore);

        Initialize();
        UserId = userId;
        SkillName = skillName.Trim();
        SkillLevel = skillLevel;
        OverallScore = overallScore;
    }

    public void Update(string skillName, int skillLevel, int overallScore)
    {
        Validate(UserId, skillName, skillLevel, overallScore);

        SkillName = skillName.Trim();
        SkillLevel = skillLevel;
        OverallScore = overallScore;
        MarkAsUpdated();
    }

    private static void Validate(Guid userId, string skillName, int skillLevel, int overallScore)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(skillName);

        if (skillName.Trim().Length > 200)
        {
            throw new ArgumentException("Skill name cannot exceed 200 characters.", nameof(skillName));
        }

        ValidateScore(skillLevel, nameof(skillLevel), "Skill level");
        ValidateScore(overallScore, nameof(overallScore), "Overall score");
    }

    private static void ValidateScore(int score, string parameterName, string displayName)
    {
        if (score is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                score,
                displayName + " must be between 0 and 100.");
        }
    }
}
