using System.Text.Json.Serialization;
using Vettingo.EvaluationService.Domain.Enums;

namespace Vettingo.EvaluationService.Application.DTOs;

public sealed record CvLanguageDto(
    string LanguageName,
    [property: JsonConverter(typeof(JsonStringEnumConverter<LanguageLevel>))] LanguageLevel LanguageLevel);
