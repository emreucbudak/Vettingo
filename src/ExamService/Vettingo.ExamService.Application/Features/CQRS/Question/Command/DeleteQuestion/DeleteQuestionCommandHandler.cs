using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Command.DeleteQuestion
{
    public class DeleteQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<DeleteQuestionCommandRequest>
    {
        public async Task Handle(DeleteQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Question not found");
            }

            questionRepository.DeleteQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
