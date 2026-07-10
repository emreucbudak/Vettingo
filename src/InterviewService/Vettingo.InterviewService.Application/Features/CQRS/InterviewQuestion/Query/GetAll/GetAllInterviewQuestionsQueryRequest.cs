using FlashMediator;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetAll
{
    public record GetAllInterviewQuestionsQueryRequest : IRequest<IEnumerable<GetAllInterviewQuestionsQueryResponse>>, ICacheableQuery
    {
        public Guid? CompanyId { get; init; }
    }
}
