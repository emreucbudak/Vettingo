using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById
{
    public record GetInterviewQuestionByIdQueryRequest : IRequest<GetInterviewQuestionByIdQueryResponse>
    {
        public Guid InterviewQuestionId { get; init; }
    }
}
