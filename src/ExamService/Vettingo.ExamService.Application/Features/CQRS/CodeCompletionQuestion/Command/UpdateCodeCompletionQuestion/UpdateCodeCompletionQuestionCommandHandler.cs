using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.UpdateCodeCompletionQuestion
{
    public class UpdateCodeCompletionQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<UpdateCodeCompletionQuestionCommandHandler> logger) : IRequestHandler<UpdateCodeCompletionQuestionCommandRequest>
    {
        public async Task Handle(UpdateCodeCompletionQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateCodeCompletionQuestionCommandHandler));
            var question = await questionRepository.GetCodeCompletionQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Kod tamamlama sorusu bulunamadı");
            }

            question.UpdateQuestion(request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.CodeSnippet, request.ExpectedAnswer);
            questionRepository.UpdateCodeCompletionQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



