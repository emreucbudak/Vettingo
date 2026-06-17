using FlashMediator;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Query.GetByExam
{
    public class GetCodeCompletionQuestionsByExamQueryRequest : IRequest<IEnumerable<GetCodeCompletionQuestionsByExamQueryResponse>>
    {
        public Guid ExamId { get; init; }
    }
}
