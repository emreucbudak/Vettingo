using FlashMediator;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll
{
    public class GetAllJobPostingsQueryRequest : IRequest<IEnumerable<GetAllJobPostingsQueryResponse>>
    {
        public Guid? CompanyId { get; init; }
    }
}
