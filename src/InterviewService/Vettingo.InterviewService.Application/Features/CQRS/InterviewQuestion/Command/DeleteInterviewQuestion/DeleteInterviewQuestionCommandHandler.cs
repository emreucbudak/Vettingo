using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.DeleteInterviewQuestion
{
    public class DeleteInterviewQuestionCommandHandler(IRepository<InterviewQuestionEntity> questionRepository, ILogger<DeleteInterviewQuestionCommandHandler> logger) : IRequestHandler<DeleteInterviewQuestionCommandRequest>
    {
        public async Task Handle(DeleteInterviewQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteInterviewQuestionCommandHandler));

            InterviewQuestionEntity question = await questionRepository.GetByIdAsync(request.InterviewQuestionId)
                ?? throw new NotFoundException("Mülakat sorusu bulunamadı");

            questionRepository.Delete(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}


