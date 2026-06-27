using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.InterviewService.Application.Repository;
using InterviewQuestionEntity = Vettingo.InterviewService.Domain.Entities.InterviewQuestion;

namespace Vettingo.InterviewService.Application.Features.CQRS.InterviewQuestion.Command.CreateInterviewQuestion
{
    public class CreateInterviewQuestionCommandHandler(IRepository<InterviewQuestionEntity> questionRepository, ILogger<CreateInterviewQuestionCommandHandler> logger) : IRequestHandler<CreateInterviewQuestionCommandRequest>
    {
        public async Task Handle(CreateInterviewQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateInterviewQuestionCommandHandler));

            InterviewQuestionEntity question = new();
            question.CreateQuestion(request.CompanyId, request.QuestionText);

            await questionRepository.AddAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}


