using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Query.GetByExam
{
    public class GetClassicQuestionsByExamQueryHandler(IQuestionRepository questionRepository, ILogger<GetClassicQuestionsByExamQueryHandler> logger) : IRequestHandler<GetClassicQuestionsByExamQueryRequest, IEnumerable<GetClassicQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetClassicQuestionsByExamQueryResponse>> Handle(GetClassicQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetClassicQuestionsByExamQueryHandler));
            var questions = await questionRepository.GetClassicQuestionsByExamIdAsync(request.ExamId);

            return questions.Select(question => new GetClassicQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Weight = question.Weight,
                DisplayOrder = question.DisplayOrder,
                Explanation = question.Explanation,
                ExpectedAnswer = question.ExpectedAnswer
            });
        }
    }
}


