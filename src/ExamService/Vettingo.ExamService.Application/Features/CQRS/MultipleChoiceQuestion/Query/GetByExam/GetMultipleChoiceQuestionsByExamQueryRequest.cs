using FlashMediator;
using Vettingo.ExamService.Application.Interfaces;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam
{
    public class GetMultipleChoiceQuestionsByExamQueryRequest : IRequest<IEnumerable<GetMultipleChoiceQuestionsByExamQueryResponse>>, ICacheableQuery
    {
        public Guid ExamId { get; init; }
        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        public GetMultipleChoiceQuestionsByExamQueryRequest(Guid examId)
        {
            ExamId = examId;
            CacheKey = ExamId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(30);
        }
    }
}
