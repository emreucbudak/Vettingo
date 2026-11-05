using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.CreateTrueFalseQuestion
{
    public class CreateTrueFalseQuestionCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<CreateTrueFalseQuestionCommandHandler> logger) : IRequestHandler<CreateTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(CreateTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateTrueFalseQuestionCommandHandler));
            var exam = await examRepository.GetByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            Domain.Entities.TrueFalseQuestion question = new();
            question.CreateQuestion(exam.Id, request.QuestionText, request.Weight, request.DisplayOrder, request.TrueAnswer, request.FalseAnswer);

            exam.AddTrueFalseQuestion(question);
            await examRepository.SaveChangesAsync();
        }
    }
}



