using FlashMediator;
using Vettingo.JobService.Application.Interfaces;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetAll
{
    public class GetAllJobPostingsQueryRequest : IRequest<IEnumerable<GetAllJobPostingsQueryResponse>>, ICacheableQuery
    {
        public Guid? CompanyId { get; init; }

        public GetAllJobPostingsQueryRequest(Guid? companyId)
        {
            CompanyId = companyId;
            CacheKey = companyId.HasValue ? $"GetAllJobPostings_{companyId}" : "GetAllJobPostings";
            ExpirationTime = TimeSpan.FromMinutes(30);
        }

        public string CacheKey { get; set ; }
        public TimeSpan ExpirationTime { get ; set; }
    }
}
