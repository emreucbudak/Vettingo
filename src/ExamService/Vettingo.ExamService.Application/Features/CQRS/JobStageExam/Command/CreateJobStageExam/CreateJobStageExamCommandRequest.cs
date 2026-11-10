using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.JobStageExam.Command.CreateJobStageExam
{
    public record CreateJobStageExamCommandRequest : IRequest
    {
        public Guid JobId { get; init; }
        public Guid StepId { get; init; }
        public Guid ExamId { get; init; }
        public DateOnly StartedDate { get; init; }
        public DateOnly EndDate { get; init; }
        public List<Guid> ExamCandidateList { get; init; } = new();
    }
}
