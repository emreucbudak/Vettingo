using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Query.GetByExam
{
    public class GetTrueFalseQuestionsByExamQueryHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<GetTrueFalseQuestionsByExamQueryHandler> logger) : IRequestHandler<GetTrueFalseQuestionsByExamQueryRequest, IEnumerable<GetTrueFalseQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetTrueFalseQuestionsByExamQueryResponse>> Handle(GetTrueFalseQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(GetTrueFalseQuestionsByExamQueryHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId);
            var questions = exam?.TrueFalseQuestions ?? [];

            return questions.OrderBy(question => question.DisplayOrder).Select(question => new GetTrueFalseQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Weight = question.Weight,
                DisplayOrder = question.DisplayOrder,
                TrueAnswer = question.TrueAnswer,
                FalseAnswer = question.FalseAnswer
            });
        }
    }
}


