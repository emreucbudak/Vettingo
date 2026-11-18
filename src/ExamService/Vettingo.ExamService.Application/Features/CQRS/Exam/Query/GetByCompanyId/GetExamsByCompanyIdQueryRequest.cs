using System.Text.Json.Serialization;
using FlashMediator;
using ICacheableQuery = Vettingo.ExamService.Application.Interfaces.ICacheableQuery;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;

public sealed class GetExamsByCompanyIdQueryRequest(Guid companyId)
    : IRequest<List<GetExamsByCompanyIdQueryResponse>>, ICacheableQuery
{
    [JsonIgnore]
    public Guid CompanyId { get; } = companyId;

    [JsonIgnore]
    public string CacheKey { get; set; } = $"{companyId}-exams";

    [JsonIgnore]
    public TimeSpan ExpirationTime { get; set; } = TimeSpan.FromMinutes(30);
}
