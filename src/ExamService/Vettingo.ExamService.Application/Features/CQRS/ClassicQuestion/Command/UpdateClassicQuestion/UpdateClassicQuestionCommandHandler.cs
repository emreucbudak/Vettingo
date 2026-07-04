using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.UpdateClassicQuestion
{
    public class UpdateClassicQuestionCommandHandler(IQuestionRepository questionRepository, ILogger<UpdateClassicQuestionCommandHandler> logger) : IRequestHandler<UpdateClassicQuestionCommandRequest>
    {
        public async Task Handle(UpdateClassicQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateClassicQuestionCommandHandler));
            var question = await questionRepository.GetClassicQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Klasik soru bulunamadı");
            }

            question.UpdateQuestion(request.QuestionText, request.Weight, request.DisplayOrder, request.Explanation, request.ExpectedAnswer);
            questionRepository.UpdateClassicQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



