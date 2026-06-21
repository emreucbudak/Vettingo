using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.DeleteClassicQuestion
{
    public class DeleteClassicQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<DeleteClassicQuestionCommandHandler> logger) : IRequestHandler<DeleteClassicQuestionCommandRequest>
    {
        public async Task Handle(DeleteClassicQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteClassicQuestionCommandHandler));
            var question = await questionRepository.GetClassicQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Klasik soru bulunamadı");
            }

            questionRepository.DeleteClassicQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



