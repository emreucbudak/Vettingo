using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.CreateCodeCompletionQuestion
{
    public class CreateCodeCompletionQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository) : IRequestHandler<CreateCodeCompletionQuestionCommandRequest>
    {
        public async Task Handle(CreateCodeCompletionQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new Exception("Exam not found");
            }

            Domain.Entities.CodeCompletionQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.CodeSnippet, request.ExpectedAnswer);

            await questionRepository.AddCodeCompletionQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
