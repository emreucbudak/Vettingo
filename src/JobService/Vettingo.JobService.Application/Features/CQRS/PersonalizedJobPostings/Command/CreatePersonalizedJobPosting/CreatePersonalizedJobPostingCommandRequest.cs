using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

public record CreatePersonalizedJobPostingCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public Guid JobPostingId { get; init; }
}
