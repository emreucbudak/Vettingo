using FlashMediator;
using ICacheableQuery = Vettingo.InterviewService.Application.Interfaces.ICacheableQuery;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById
{
    public record GetInterviewQuestionByIdQueryRequest : IRequest<GetInterviewQuestionByIdQueryResponse>, ICacheableQuery
    {
        public Guid InterviewQuestionId { get; init; }
        public string CacheKey => $"GetInterviewQuestionById_{InterviewQuestionId}";
        public TimeSpan ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
