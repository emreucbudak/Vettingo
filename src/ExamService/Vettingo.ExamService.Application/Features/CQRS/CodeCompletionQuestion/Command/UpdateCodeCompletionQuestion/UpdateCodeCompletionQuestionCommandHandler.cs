using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.CodeCompletionQuestion.Command.UpdateCodeCompletionQuestion
{
    public class UpdateCodeCompletionQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<UpdateCodeCompletionQuestionCommandRequest>
    {
        public async Task Handle(UpdateCodeCompletionQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetCodeCompletionQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Code completion question not found");
            }

            question.UpdateQuestion(request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.CodeSnippet, request.ExpectedAnswer);
            questionRepository.UpdateCodeCompletionQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
