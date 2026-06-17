using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Query.GetByExam
{
    public class GetClassicQuestionsByExamQueryRequest : IRequest<IEnumerable<GetClassicQuestionsByExamQueryResponse>>
    {
        public Guid ExamId { get; init; }
    }
}
