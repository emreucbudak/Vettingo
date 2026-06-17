using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Query.GetByExam
{
    public class GetCodeCompletionQuestionsByExamQueryHandler(IQuestionRepository questionRepository) : IRequestHandler<GetCodeCompletionQuestionsByExamQueryRequest, IEnumerable<GetCodeCompletionQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetCodeCompletionQuestionsByExamQueryResponse>> Handle(GetCodeCompletionQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            var questions = await questionRepository.GetCodeCompletionQuestionsByExamIdAsync(request.ExamId);

            return questions.Select(question => new GetCodeCompletionQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Topic = question.Topic,
                Point = question.Point,
                Weight = question.Weight,
                DisplayOrder = question.DisplayOrder,
                Explanation = question.Explanation,
                CodeSnippet = question.CodeSnippet,
                ExpectedAnswer = question.ExpectedAnswer
            });
        }
    }
}
