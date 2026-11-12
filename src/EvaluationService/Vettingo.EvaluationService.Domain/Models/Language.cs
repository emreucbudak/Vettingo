using Vettingo.EvaluationService.Domain.Common;
using Vettingo.EvaluationService.Domain.Enums;

namespace Vettingo.EvaluationService.Domain.Models;

public sealed class Language : BaseEntity
{
    private Language()
    {
    }

    public Language(string languageName, LanguageLevel languageLevel)
    {
        Initialize();
        LanguageName = languageName;
        LanguageLevel = languageLevel;
    }

    public string LanguageName { get; private set; } = string.Empty;
    public LanguageLevel LanguageLevel { get; private set; }
}
