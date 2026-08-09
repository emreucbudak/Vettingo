using FlashMediator;
using ICacheableQuery = Vettingo.InterviewService.Application.Interfaces.ICacheableQuery;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll
{
    public record GetAllInterviewExamsQueryRequest : IRequest<IEnumerable<GetAllInterviewExamsQueryResponse>>, ICacheableQuery
    {
        public Guid? CompanyId { get; init; }
        public Guid? CandidateId { get; init; }
        public bool UpcomingOnly { get; init; }
        public string CacheKey => $"GetAllInterviewExams_{CompanyId}_{CandidateId}_{UpcomingOnly}";
        public TimeSpan ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
