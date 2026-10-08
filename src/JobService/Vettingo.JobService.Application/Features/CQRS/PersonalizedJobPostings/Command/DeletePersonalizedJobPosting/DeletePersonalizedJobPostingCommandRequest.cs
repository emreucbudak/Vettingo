using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.DeletePersonalizedJobPosting;

public record DeletePersonalizedJobPostingCommandRequest : IRequest
{
    public Guid PersonalizedJobPostingId { get; init; }
}
