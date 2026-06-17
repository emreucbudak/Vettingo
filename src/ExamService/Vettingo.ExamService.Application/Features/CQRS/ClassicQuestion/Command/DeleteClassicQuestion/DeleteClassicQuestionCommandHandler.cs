using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.DeleteClassicQuestion
{
    public class DeleteClassicQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<DeleteClassicQuestionCommandRequest>
    {
        public async Task Handle(DeleteClassicQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetClassicQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Classic question not found");
            }

            questionRepository.DeleteClassicQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
