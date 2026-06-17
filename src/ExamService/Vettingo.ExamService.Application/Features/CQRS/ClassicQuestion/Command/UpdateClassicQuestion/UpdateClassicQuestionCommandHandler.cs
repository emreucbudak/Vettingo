using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.ClassicQuestion.Command.UpdateClassicQuestion
{
    public class UpdateClassicQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<UpdateClassicQuestionCommandRequest>
    {
        public async Task Handle(UpdateClassicQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetClassicQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("Classic question not found");
            }

            question.UpdateQuestion(request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.ExpectedAnswer);
            questionRepository.UpdateClassicQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
