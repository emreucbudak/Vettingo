using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam
{
    public class GetMultipleChoiceQuestionsByExamQueryRequest : IRequest<IEnumerable<GetMultipleChoiceQuestionsByExamQueryResponse>>
    {
        public Guid ExamId { get; init; }
    }
}
