using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Command.CloseJobPostingApplications
{
    public record CloseJobPostingApplicationsCommandRequest : IRequest
    {
        public Guid JobPostingId { get; init; }
    }
}
