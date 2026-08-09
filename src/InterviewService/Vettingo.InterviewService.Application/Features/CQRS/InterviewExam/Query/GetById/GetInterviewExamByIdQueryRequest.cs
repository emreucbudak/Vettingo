using FlashMediator;
using ICacheableQuery = Vettingo.InterviewService.Application.Interfaces.ICacheableQuery;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetById
{
    public record GetInterviewExamByIdQueryRequest : IRequest<GetInterviewExamByIdQueryResponse>, ICacheableQuery
    {
        public Guid InterviewExamId { get; init; }
        public string CacheKey => $"GetInterviewExamById_{InterviewExamId}";
        public TimeSpan ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
