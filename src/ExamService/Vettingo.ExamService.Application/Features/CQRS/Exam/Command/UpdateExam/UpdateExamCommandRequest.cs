using FlashMediator;
using Vettingo.ExamService.Domain.Enums;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.UpdateExam
{
    public record UpdateExamCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
        public Guid? CompanyId { get; init; }
        public Guid? JobId { get; init; }
        public ExamOwnerType OwnerType { get; init; } = ExamOwnerType.System;
        public string Title { get; init; } = string.Empty;
        public string Subject { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public int PassingScore { get; init; }
        public bool IsActive { get; init; }
    }
}
