using FlashMediator;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetAll
{
    public record GetAllInterviewExamsQueryRequest : IRequest<IEnumerable<GetAllInterviewExamsQueryResponse>>, ICacheableQuery
    {
        public Guid? CompanyId { get; init; }
    }
}
