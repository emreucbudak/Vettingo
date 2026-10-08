using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Command.CreatePersonalizedJobPosting;

public record CreatePersonalizedJobPostingCommandRequest : IRequest
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateOnly PublishedDate { get; init; }
    public int CityId { get; init; }
}
