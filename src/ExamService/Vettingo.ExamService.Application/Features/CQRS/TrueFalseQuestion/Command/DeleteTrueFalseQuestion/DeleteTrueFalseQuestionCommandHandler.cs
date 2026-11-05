using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion
{
    public class DeleteTrueFalseQuestionCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<DeleteTrueFalseQuestionCommandHandler> logger) : IRequestHandler<DeleteTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(DeleteTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteTrueFalseQuestionCommandHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId, "MultipleChoiceQuestions.Options", "TrueFalseQuestions")
                ?? throw new NotFoundException("Sınav bulunamadı");

            if (!exam.RemoveTrueFalseQuestion(request.QuestionId))
            {
                throw new NotFoundException("Doğru yanlış sorusu bulunamadı");
            }

            await examRepository.SaveChangesAsync();
        }
    }
}



