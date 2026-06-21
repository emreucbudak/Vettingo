using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam
{
    public class GetTrueFalseQuestionsByExamQueryHandler(IQuestionRepository questionRepository, ILogger<GetTrueFalseQuestionsByExamQueryHandler> logger) : IRequestHandler<GetTrueFalseQuestionsByExamQueryRequest, IEnumerable<GetTrueFalseQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetTrueFalseQuestionsByExamQueryResponse>> Handle(GetTrueFalseQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetTrueFalseQuestionsByExamQueryHandler));
            var questions = await questionRepository.GetTrueFalseQuestionsByExamIdAsync(request.ExamId);

            return questions.Select(question => new GetTrueFalseQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Topic = question.Topic,
                Point = question.Point,
                Weight = question.Weight,
                DisplayOrder = question.DisplayOrder,
                Explanation = question.Explanation,
                CorrectAnswer = question.CorrectAnswer
            });
        }
    }
}


