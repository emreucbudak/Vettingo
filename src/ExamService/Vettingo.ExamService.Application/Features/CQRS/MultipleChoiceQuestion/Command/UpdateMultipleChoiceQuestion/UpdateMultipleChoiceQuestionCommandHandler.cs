using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion
{
    public class UpdateMultipleChoiceQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<UpdateMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(UpdateMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetMultipleChoiceQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Multiple choice question not found");
            }

            question.UpdateQuestion(request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation);
            question.ClearOptions();

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.MultipleChoiceOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.DisplayOrder);
                question.AddOption(option);
            }

            questionRepository.UpdateMultipleChoiceQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
