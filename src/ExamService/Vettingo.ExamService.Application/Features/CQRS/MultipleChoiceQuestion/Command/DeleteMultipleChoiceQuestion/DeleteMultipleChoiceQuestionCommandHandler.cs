using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.DeleteMultipleChoiceQuestion
{
    public class DeleteMultipleChoiceQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<DeleteMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(DeleteMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetMultipleChoiceQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Multiple choice question not found");
            }

            questionRepository.DeleteMultipleChoiceQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
