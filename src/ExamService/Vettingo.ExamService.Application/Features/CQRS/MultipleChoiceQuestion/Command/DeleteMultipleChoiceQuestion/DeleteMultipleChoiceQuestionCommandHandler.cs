using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion
{
    public class DeleteMultipleChoiceQuestionCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<DeleteMultipleChoiceQuestionCommandHandler> logger) : IRequestHandler<DeleteMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(DeleteMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(DeleteMultipleChoiceQuestionCommandHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId)
                ?? throw new NotFoundException("Sınav bulunamadı");

            if (!exam.RemoveMultipleChoiceQuestion(request.QuestionId))
            {
                throw new NotFoundException("Çoktan seçmeli soru bulunamadı");
            }

            await examRepository.SaveChangesAsync();
        }
    }
}



