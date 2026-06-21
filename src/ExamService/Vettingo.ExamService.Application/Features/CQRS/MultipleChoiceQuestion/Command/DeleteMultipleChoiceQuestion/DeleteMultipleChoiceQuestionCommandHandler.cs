using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion
{
    public class DeleteMultipleChoiceQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<DeleteMultipleChoiceQuestionCommandHandler> logger) : IRequestHandler<DeleteMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(DeleteMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteMultipleChoiceQuestionCommandHandler));
            var question = await questionRepository.GetMultipleChoiceQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Çoktan seçmeli soru bulunamadı");
            }

            questionRepository.DeleteMultipleChoiceQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



