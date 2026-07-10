using FlashMediator;
using Vettingo.JobService.Application.Interfaces;

namespace Vettingo.JobService.Application.Features.CQRS.JobPosting.Query.GetById
{
    public class GetJobPostingByIdQueryRequest : IRequest<GetJobPostingByIdQueryResponse>, ICacheableQuery
    {
        public Guid JobPostingId { get; init; }
        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        public GetJobPostingByIdQueryRequest(Guid jobPostingId)
        {
            JobPostingId = jobPostingId;
            CacheKey = JobPostingId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(30);
        }
    }
}
