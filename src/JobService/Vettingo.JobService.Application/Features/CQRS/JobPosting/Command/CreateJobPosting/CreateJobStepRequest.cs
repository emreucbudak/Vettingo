using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CreateJobPosting;

public record CreateJobStepRequest
{
    public int Order { get; init; }
    public StepType StepType { get; init; }
    public Guid? ExamId { get; init; }
    public DateTime StartedDate { get; init; }
    public DateTime EndDate { get; init; }
}
