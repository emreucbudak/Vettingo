using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.UpdateTrueFalseQuestion
{
    public class UpdateTrueFalseQuestionCommandHandler(IQuestionRepository questionRepository) : IRequestHandler<UpdateTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(UpdateTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var question = await questionRepository.GetTrueFalseQuestionByIdAsync(request.QuestionId);

            if (question is null)
            {
                throw new Exception("True false question not found");
            }

            question.UpdateQuestion(request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.CorrectAnswer);
            questionRepository.UpdateTrueFalseQuestion(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
