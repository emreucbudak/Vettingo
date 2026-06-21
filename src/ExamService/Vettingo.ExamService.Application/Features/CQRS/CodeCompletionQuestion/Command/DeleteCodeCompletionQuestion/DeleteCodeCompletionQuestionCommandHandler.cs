using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.DeleteCodeCompletionQuestion
{
    public class DeleteCodeCompletionQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<DeleteCodeCompletionQuestionCommandHandler> logger) : IRequestHandler<DeleteCodeCompletionQuestionCommandRequest>
    {
        public async Task Handle(DeleteCodeCompletionQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteCodeCompletionQuestionCommandHandler));
            var question = await questionRepository.GetCodeCompletionQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Kod tamamlama sorusu bulunamadı");
            }

            questionRepository.DeleteCodeCompletionQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



