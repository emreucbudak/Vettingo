using FlashMediator;

namespace Vettingo.ApplicationService.Application.Features.CQRS.JobApplication.Query.GetAll
{
    public record GetJobApplicationsQueryRequest : IRequest<IEnumerable<GetJobApplicationsQueryResponse>>
    {
        public Guid? CandidateId { get; init; }
        public Guid? JobPostingId { get; init; }
    }
}
