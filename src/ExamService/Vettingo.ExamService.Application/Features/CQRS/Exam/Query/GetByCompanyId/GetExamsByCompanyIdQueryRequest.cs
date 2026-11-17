using System.Text.Json.Serialization;
using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetByCompanyId;

public sealed class GetExamsByCompanyIdQueryRequest : IRequest<List<GetExamsByCompanyIdQueryResponse>>
{
    [JsonIgnore]
    public Guid CompanyId { get; init; }
}
