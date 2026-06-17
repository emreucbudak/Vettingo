using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.MultipleChoiceQuestion.Command.CreateMultipleChoiceQuestion
{
    public class CreateMultipleChoiceQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository) : IRequestHandler<CreateMultipleChoiceQuestionCommandRequest>
    {
        public async Task Handle(CreateMultipleChoiceQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new Exception("Exam not found");
            }

            Domain.Entities.MultipleChoiceQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation);

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.MultipleChoiceOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.DisplayOrder);
                question.AddOption(option);
            }

            await questionRepository.AddMultipleChoiceQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
