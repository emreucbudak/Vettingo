using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion
{
    public class UpdateMultipleChoiceQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<UpdateMultipleChoiceQuestionCommandHandler> logger) : IRequestHandler<UpdateMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(UpdateMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateMultipleChoiceQuestionCommandHandler));
            var question = await questionRepository.GetMultipleChoiceQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Çoktan seçmeli soru bulunamadı");
            }

            question.UpdateQuestion(request.QuestionText, request.Weight, request.DisplayOrder, request.Explanation);
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



