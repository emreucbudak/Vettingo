using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetToday
{
    public record GetTodayInterviewsQueryRequest(Guid CompanyId) : IRequest<IEnumerable<GetTodayInterviewsQueryResponse>>;
}
