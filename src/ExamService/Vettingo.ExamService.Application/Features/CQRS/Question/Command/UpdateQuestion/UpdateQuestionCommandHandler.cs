using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Command.UpdateQuestion
{
    public class UpdateQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<UpdateQuestionCommandRequest>
    {
        public async Task Handle(UpdateQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Question not found");
            }

            question.UpdateQuestion(request.QuestionText, request.Topic, request.QuestionType, request.Point, request.DisplayOrder, request.Explanation);
            question.ClearOptions();

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.QuestionOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.DisplayOrder);
                question.AddOption(option);
            }

            questionRepository.UpdateQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
