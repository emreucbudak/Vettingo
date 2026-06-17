using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Query.GetByExam
{
    public class GetMultipleChoiceQuestionsByExamQueryHandler(IQuestionRepository questionRepository) : IRequestHandler<GetMultipleChoiceQuestionsByExamQueryRequest, IEnumerable<GetMultipleChoiceQuestionsByExamQueryResponse>>
    {
        public async Task<IEnumerable<GetMultipleChoiceQuestionsByExamQueryResponse>> Handle(GetMultipleChoiceQuestionsByExamQueryRequest request, CancellationToken cancellationToken)
        {
            var questions = await questionRepository.GetMultipleChoiceQuestionsByExamIdAsync(request.ExamId);

            return questions.Select(question => new GetMultipleChoiceQuestionsByExamQueryResponse
            {
                Id = question.Id,
                ExamId = question.ExamId,
                QuestionText = question.QuestionText,
                Topic = question.Topic,
                Point = question.Point,
                Weight = question.Weight,
                DisplayOrder = question.DisplayOrder,
                Explanation = question.Explanation,
                Options = question.Options
                    .OrderBy(option => option.DisplayOrder)
                    .Select(option => new GetMultipleChoiceQuestionOptionResponse
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
