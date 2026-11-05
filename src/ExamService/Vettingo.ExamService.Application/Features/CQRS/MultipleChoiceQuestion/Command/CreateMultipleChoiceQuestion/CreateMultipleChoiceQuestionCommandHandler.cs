using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion
{
    public class CreateMultipleChoiceQuestionCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<CreateMultipleChoiceQuestionCommandHandler> logger) : IRequestHandler<CreateMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(CreateMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateMultipleChoiceQuestionCommandHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId, "MultipleChoiceQuestions.Options", "TrueFalseQuestions");

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            Domain.Entities.MultipleChoiceQuestion question = new();
            question.CreateQuestion(exam.Id, request.QuestionText, request.Weight, request.DisplayOrder);

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.MultipleChoiceOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect);
                question.AddOption(option);
            }

            exam.AddMultipleChoiceQuestion(question);
            await examRepository.SaveChangesAsync();
        }
    }
}



