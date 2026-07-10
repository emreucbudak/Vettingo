using FlashMediator;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Query.GetAll
{
    public record GetInterviewAnswersQueryRequest : IRequest<IEnumerable<GetInterviewAnswersQueryResponse>>, ICacheableQuery
    {
        public GetInterviewAnswersQueryRequest(Guid? userId, Guid? ınterviewExamId)
        {
            UserId = userId;
            InterviewExamId = ınterviewExamId;
            CacheKey = $"GetInterviewAnswers_{UserId}_{InterviewExamId}";
            ExpirationTime = TimeSpan.FromMinutes(10);
        }

        public Guid? UserId { get; init; }
        public Guid? InterviewExamId { get; init; }
        public string CacheKey { get; set; } 
        public TimeSpan ExpirationTime { get; set; }
    }
}
