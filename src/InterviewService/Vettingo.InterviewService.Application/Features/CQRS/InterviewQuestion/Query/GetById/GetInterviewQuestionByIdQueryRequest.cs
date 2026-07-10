using FlashMediator;
using Vettingo.InterviewService.Application.Interfaces;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Query.GetById
{
    public record GetInterviewQuestionByIdQueryRequest : IRequest<GetInterviewQuestionByIdQueryResponse>, ICacheableQuery
    {
        public Guid InterviewQuestionId { get; init; }
    }
}
