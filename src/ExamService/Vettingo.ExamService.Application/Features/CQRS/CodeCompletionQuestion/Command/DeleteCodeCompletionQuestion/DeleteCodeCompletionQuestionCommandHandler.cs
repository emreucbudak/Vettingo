using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.DeleteCodeCompletionQuestion
{
    public class DeleteCodeCompletionQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<DeleteCodeCompletionQuestionCommandRequest>
    {
        public async Task Handle(DeleteCodeCompletionQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetCodeCompletionQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Code completion question not found");
            }

            questionRepository.DeleteCodeCompletionQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
