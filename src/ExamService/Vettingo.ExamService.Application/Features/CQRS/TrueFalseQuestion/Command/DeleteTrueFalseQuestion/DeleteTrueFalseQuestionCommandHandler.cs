using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.DeleteTrueFalseQuestion
{
    public class DeleteTrueFalseQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<DeleteTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(DeleteTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetTrueFalseQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("True false question not found");
            }

            questionRepository.DeleteTrueFalseQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
