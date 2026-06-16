using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Query.GetById
{
    public class GetExamByIdQueryRequest : IRequest<GetExamByIdQueryResponse>
    {
        public Guid ExamId { get; init; }
    }
}
