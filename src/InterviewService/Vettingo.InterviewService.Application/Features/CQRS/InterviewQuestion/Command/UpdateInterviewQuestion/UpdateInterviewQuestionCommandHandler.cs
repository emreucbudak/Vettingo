using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Exceptions;
using Vettingo.InterviewService.Application.Repository;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.UpdateInterviewQuestion
{
    public class UpdateInterviewQuestionCommandHandler(IRepository<InterviewQuestionEntity> questionRepository, ILogger<UpdateInterviewQuestionCommandHandler> logger) : IRequestHandler<UpdateInterviewQuestionCommandRequest>
    {
        public async Task Handle(UpdateInterviewQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateInterviewQuestionCommandHandler));

            InterviewQuestionEntity question = await questionRepository.GetByIdAsync(request.InterviewQuestionId)
                ?? throw new NotFoundException("Mülakat sorusu bulunamadı");

            question.UpdateQuestion(request.CompanyId, request.QuestionText);
            questionRepository.Update(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}


