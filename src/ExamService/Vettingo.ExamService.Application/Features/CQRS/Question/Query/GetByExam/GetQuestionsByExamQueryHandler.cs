using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Query.GetByExam
{
    public class GetQuestionsByExamQueryHandler(IQuestionRepository questionRepository) : IRequestHandler<GetQuestionsByExamQueryRequest, IEnumerable<GetQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetQuestionsByExamQueryResponse>> Handle(GetQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            var questions = await questionRepository.GetQuestionsByExamIdAsync(request.ExamId);

            return questions.Select(question => new GetQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Topic = question.Topic,
                QuestionType = question.QuestionType,
                Point = question.Point,
                DisplayOrder = question.DisplayOrder,
                Explanation = question.Explanation,
                Options = question.Options
                    .OrderBy(option => option.DisplayOrder)
                    .Select(option => new GetQuestionsByExamOptionResponse
                    {
                        Id = option.Id,
                        OptionText = option.OptionText,
                        IsCorrect = option.IsCorrect,
                        DisplayOrder = option.DisplayOrder
                    })
                    .ToList()
            });
        }
    }
}
