using FlashMediator;
using Vettingo.ExamService.Application.Interfaces;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetAll
{
    public class GetAllExamsQueryRequest : IRequest<IEnumerable<GetAllExamsQueryResponse>>, ICacheableQuery
    {
        public GetAllExamsQueryRequest()
        {
            CacheKey = "GetAllExams";
            ExpirationTime = TimeSpan.FromMinutes(30);
        }

        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
