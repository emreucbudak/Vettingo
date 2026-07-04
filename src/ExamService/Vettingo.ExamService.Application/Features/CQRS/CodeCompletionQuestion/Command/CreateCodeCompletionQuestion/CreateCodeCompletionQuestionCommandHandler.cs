using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.CreateCodeCompletionQuestion
{
    public class CreateCodeCompletionQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository, ILogger<CreateCodeCompletionQuestionCommandHandler> logger) : IRequestHandler<CreateCodeCompletionQuestionCommandRequest>
    {
        public async Task Handle(CreateCodeCompletionQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateCodeCompletionQuestionCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            Domain.Entities.CodeCompletionQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Weight, request.DisplayOrder, request.Explanation, request.CodeSnippet, request.ExpectedAnswer);

            await questionRepository.AddCodeCompletionQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



