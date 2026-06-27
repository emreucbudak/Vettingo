using FlashMediator;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewExam.Query.GetById
{
    public record GetInterviewExamByIdQueryRequest : IRequest<GetInterviewExamByIdQueryResponse>
    {
        public Guid InterviewExamId { get; init; }
    }
}
