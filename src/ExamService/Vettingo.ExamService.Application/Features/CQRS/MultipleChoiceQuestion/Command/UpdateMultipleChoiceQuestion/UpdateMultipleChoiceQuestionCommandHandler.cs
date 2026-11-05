using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.UpdateMultipleChoiceQuestion
{
    public class UpdateMultipleChoiceQuestionCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<UpdateMultipleChoiceQuestionCommandHandler> logger) : IRequestHandler<UpdateMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(UpdateMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateMultipleChoiceQuestionCommandHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId, "MultipleChoiceQuestions.Options", "TrueFalseQuestions")
                ?? throw new NotFoundException("Sınav bulunamadı");
            var question = exam.MultipleChoiceQuestions.FirstOrDefault(question => question.Id == request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Çoktan seçmeli soru bulunamadı");
            }

            question.UpdateQuestion(request.QuestionText, request.Weight, request.DisplayOrder);
            question.ClearOptions();

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.MultipleChoiceOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect);
                question.AddOption(option);
            }

            await examRepository.SaveChangesAsync();
        }
    }
}



