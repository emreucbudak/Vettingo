using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Query.GetByExam
{
    public class GetCodeCompletionQuestionsByExamQueryHandler(IQuestionRepository questionRepository, ILogger<GetCodeCompletionQuestionsByExamQueryHandler> logger) : IRequestHandler<GetCodeCompletionQuestionsByExamQueryRequest, IEnumerable<GetCodeCompletionQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetCodeCompletionQuestionsByExamQueryResponse>> Handle(GetCodeCompletionQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetCodeCompletionQuestionsByExamQueryHandler));
            var questions = await questionRepository.GetCodeCompletionQuestionsByExamIdAsync(request.ExamId);

            return questions.Select(question => new GetCodeCompletionQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Weight = question.Weight,
                DisplayOrder = question.DisplayOrder,
                Explanation = question.Explanation,
                CodeSnippet = question.CodeSnippet,
                ExpectedAnswer = question.ExpectedAnswer
            });
        }
    }
}


