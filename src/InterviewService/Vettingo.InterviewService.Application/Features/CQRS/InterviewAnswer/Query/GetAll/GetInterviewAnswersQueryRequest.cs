using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewAnswer.Query.GetAll
{
    public record GetInterviewAnswersQueryRequest : IRequest<IEnumerable<GetInterviewAnswersQueryResponse>>
    {
        public Guid? UserId { get; init; }
        public Guid? InterviewExamId { get; init; }
    }
}
