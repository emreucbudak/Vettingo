using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Query.GetByExam
{
    public class GetQuestionsByExamQueryRequest : IRequest<IEnumerable<GetQuestionsByExamQueryResponse>>
    {
        public Guid ExamId { get; init; }
    }
}
