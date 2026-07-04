using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion
{
    public class CreateMultipleChoiceQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository, ILogger<CreateMultipleChoiceQuestionCommandHandler> logger) : IRequestHandler<CreateMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(CreateMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateMultipleChoiceQuestionCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            Domain.Entities.MultipleChoiceQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Weight, request.DisplayOrder, request.Explanation);

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.MultipleChoiceOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.DisplayOrder);
                question.AddOption(option);
            }

            await questionRepository.AddMultipleChoiceQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



