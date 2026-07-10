using FlashMediator;
using Vettingo.ExamService.Application.Interfaces;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Query.GetByExam
{
    public class GetClassicQuestionsByExamQueryRequest : IRequest<IEnumerable<GetClassicQuestionsByExamQueryResponse>>, ICacheableQuery
    {
        public Guid ExamId { get; init; }
        public string CacheKey { get => CacheKey; set => ExamId.ToString(); }
        public TimeSpan ExpirationTime { get => ExpirationTime; set => TimeSpan.FromMinutes(30); }
    }
}
