using FlashMediator;
using Vettingo.ExamService.Application.Interfaces;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public class GetExamByIdQueryRequest : IRequest<GetExamByIdQueryResponse>, ICacheableQuery
    {
        public Guid ExamId { get; init; }

        public GetExamByIdQueryRequest(Guid examId)
        {
            ExamId = examId;
            CacheKey = ExamId.ToString();
            ExpirationTime = TimeSpan.FromMinutes(30);
        }

        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
