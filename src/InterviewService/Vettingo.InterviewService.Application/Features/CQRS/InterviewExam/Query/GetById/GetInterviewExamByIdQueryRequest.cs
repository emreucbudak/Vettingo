using FlashMediator;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetById
{
    public record GetInterviewExamByIdQueryRequest : IRequest<GetInterviewExamByIdQueryResponse>, ICacheableQuery
    {
        public Guid InterviewExamId { get; init; }
        public string CacheKey => $"GetInterviewExamById_{InterviewExamId}";
        public TimeSpan ExpirationTime => TimeSpan.FromMinutes(10);
    }
}
