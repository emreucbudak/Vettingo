using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetAll
{
    public record GetAllInterviewQuestionsQueryRequest : IRequest<IEnumerable<GetAllInterviewQuestionsQueryResponse>>
    {
        public Guid? CompanyId { get; init; }
    }
}
