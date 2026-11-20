using FlashMediator;
using System.Text.Json.Serialization;

namespace Vettingo.JobService.Application.Features.CQRS.PersonalizedJobPostings.Query.GetAll;

public record GetAllPersonalizedJobPostingsQueryRequest : IRequest<IEnumerable<GetAllPersonalizedJobPostingsQueryResponse>>
{
    [JsonIgnore]
    public Guid UserId { get; init; }
}
