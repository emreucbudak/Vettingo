using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CloseJobPosting
{
    public record CloseJobPostingCommandRequest : IRequest
    {
        public Guid JobPostingId { get; init; }
    }
}
