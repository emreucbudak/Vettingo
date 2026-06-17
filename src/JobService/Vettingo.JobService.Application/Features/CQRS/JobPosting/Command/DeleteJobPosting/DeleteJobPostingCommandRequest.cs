using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.DeleteJobPosting
{
    public record DeleteJobPostingCommandRequest : IRequest
    {
        public Guid JobPostingId { get; init; }
    }
}
