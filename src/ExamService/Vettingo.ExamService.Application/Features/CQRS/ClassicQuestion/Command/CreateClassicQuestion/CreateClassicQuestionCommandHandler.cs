using FlashMediator;
using Vettingo.ExamService.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.CreateClassicQuestion
{
    public class CreateClassicQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository, ILogger<CreateClassicQuestionCommandHandler> logger) : IRequestHandler<CreateClassicQuestionCommandRequest>
    {
        public async Task Handle(CreateClassicQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateClassicQuestionCommandHandler));
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new NotFoundException("Sınav bulunamadı");
            }

            Domain.Entities.ClassicQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.ExpectedAnswer);

            await questionRepository.AddClassicQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}



