using FlashMediator;
using Vettingo.JobService.Domain.Enums;

namespace Vettingo.JobService.Application.Features.CQRS.JobApplication.Command.Create
{
    public record CreateJobApplicationCommandRequest : IRequest<CreateJobApplicationCommandResponse>
    {
        public Guid CandidateId { get; init; }
        public Guid JobPostingId { get; init; }
        public DateTime? AppliedAt { get; init; }
        public ApplicationStatus Status { get; init; } = ApplicationStatus.Submitted;
    }

    public record CreateJobApplicationCommandResponse(Guid Id);
}
