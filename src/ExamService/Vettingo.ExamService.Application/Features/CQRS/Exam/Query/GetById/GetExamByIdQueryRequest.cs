using System.Text.Json.Serialization;
using FlashMediator;
using ICacheableQuery = Vettingo.ExamService.Application.Interfaces.ICacheableQuery;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public class GetExamByIdQueryRequest : IRequest<GetExamByIdQueryResponse>, ICacheableQuery
    {
        public Guid ExamId { get; }

        [JsonIgnore]
        public Guid CompanyId { get; }

        public GetExamByIdQueryRequest(Guid examId, Guid companyId)
        {
            ExamId = examId;
            CompanyId = companyId;
            CacheKey = $"{CompanyId}:{ExamId}";
            ExpirationTime = TimeSpan.FromMinutes(30);
        }

        public string CacheKey { get; set; }
        public TimeSpan ExpirationTime { get; set; }
    }
}
