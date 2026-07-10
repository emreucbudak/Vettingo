using FlashMediator;
using Vettingo.ExamService.Application.Interfaces;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Query.GetByExam
{
    public class GetCodeCompletionQuestionsByExamQueryRequest : IRequest<IEnumerable<GetCodeCompletionQuestionsByExamQueryResponse>>, ICacheableQuery
    {
        public Guid ExamId { get; init; }
        public string CacheKey { get ; set ; }
        public TimeSpan ExpirationTime { get ; set; }

        public GetCodeCompletionQuestionsByExamQueryRequest(Guid examId)
        {
            ExamId = examId;
            CacheKey = ExamId.ToString();
            ExpirationTime = TimeSpan.Zero;

        }
    }
}
