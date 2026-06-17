using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById
{
    public class GetJobPostingByIdQueryRequest : IRequest<GetJobPostingByIdQueryResponse>
    {
        public Guid JobPostingId { get; init; }
    }
}
