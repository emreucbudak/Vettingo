using FlashMediator;
using Vettingo.ExamService.Application.Interfaces;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam
{
    public class GetTrueFalseQuestionsByExamQueryRequest : IRequest<IEnumerable<GetTrueFalseQuestionsByExamQueryResponse>>, ICacheableQuery
    {
        public Guid ExamId { get; init; }
        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }

        public GetTrueFalseQuestionsByExamQueryRequest(Guid examId)
        {
            ExamId = examId;
            CacheKey = ExamId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(30);
        }
    }
}
