using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.Interview.Query.GetByDate;

public sealed record GetInterviewsByDateQueryRequest(Guid CompanyId, DateOnly Date)
    : IRequest<IEnumerable<GetInterviewsByDateQueryResponse>>;
