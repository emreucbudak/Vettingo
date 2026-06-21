using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion
{
    public class DeleteTrueFalseQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<DeleteTrueFalseQuestionCommandHandler> logger) : IRequestHandler<DeleteTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(DeleteTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteTrueFalseQuestionCommandHandler));
            var question = await questionRepository.GetTrueFalseQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Doğru yanlış sorusu bulunamadı");
            }

            questionRepository.DeleteTrueFalseQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



