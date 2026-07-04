using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.CreateTrueFalseQuestion
{
    public class CreateTrueFalseQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository, ILogger<CreateTrueFalseQuestionCommandHandler> logger) : IRequestHandler<CreateTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(CreateTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateTrueFalseQuestionCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            Domain.Entities.TrueFalseQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Weight, request.DisplayOrder, request.Explanation, request.CorrectAnswer);

            await questionRepository.AddTrueFalseQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



