using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.DeleteExam
{
    public record DeleteExamCommandRequest : IRequest
    {
        public Guid ExamId { get; init; }
    }
}
