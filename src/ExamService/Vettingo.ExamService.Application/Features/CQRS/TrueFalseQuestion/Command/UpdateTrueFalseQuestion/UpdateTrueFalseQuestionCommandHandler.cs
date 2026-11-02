using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.UpdateTrueFalseQuestion
{
    public class UpdateTrueFalseQuestionCommandHandler(IExamRepository examRepository, ILogger<UpdateTrueFalseQuestionCommandHandler> logger) : IRequestHandler<UpdateTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(UpdateTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(UpdateTrueFalseQuestionCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId)
                ?? throw new NotFoundException("Sınav bulunamadı");
            var question = exam.TrueFalseQuestions.FirstOrDefault(question => question.Id == request.QuestionId);

            if (question is null)
            {
                throw new NotFoundException("Doğru yanlış sorusu bulunamadı");
            }

            question.UpdateQuestion(request.QuestionText, request.Weight, request.DisplayOrder, request.TrueAnswer, request.FalseAnswer);
            await examRepository.SaveChangesAsync();
        }
    }
}



