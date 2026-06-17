using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam
{
    public class GetTrueFalseQuestionsByExamQueryRequest : IRequest<IEnumerable<GetTrueFalseQuestionsByExamQueryResponse>>
    {
        public Guid ExamId { get; init; }
    }
}
