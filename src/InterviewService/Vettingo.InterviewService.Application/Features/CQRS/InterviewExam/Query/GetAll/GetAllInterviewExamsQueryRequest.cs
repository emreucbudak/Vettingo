using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll
{
    public record GetAllInterviewExamsQueryRequest : IRequest<IEnumerable<GetAllInterviewExamsQueryResponse>>
    {
        public Guid? CompanyId { get; init; }
    }
}
