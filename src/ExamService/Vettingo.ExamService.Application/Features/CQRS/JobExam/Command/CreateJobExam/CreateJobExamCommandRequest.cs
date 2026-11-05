using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.JobExam.Command.CreateJobExam
{
    public record CreateJobExamCommandRequest : IRequest
    {
        public Guid JobId { get; init; }
        public Guid ExamId { get; init; }
    }
}
