using System.Text.Json.Serialization;
using FlashMediator;
using Vettingo.ExamService.Application.DTOs;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam
{
    public record UpdateExamCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
        [JsonIgnore]
        public Guid CompanyId { get; init; }
        public ExamOwnerType OwnerType { get; init; } = ExamOwnerType.System;
        public string Title { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public int PassingScore { get; init; }
        public IReadOnlyCollection<UpdateExamMultipleChoiceQuestionDto>? MultipleChoiceQuestions { get; init; }
        public IReadOnlyCollection<UpdateExamTrueFalseQuestionDto>? TrueFalseQuestions { get; init; }
    }
}
