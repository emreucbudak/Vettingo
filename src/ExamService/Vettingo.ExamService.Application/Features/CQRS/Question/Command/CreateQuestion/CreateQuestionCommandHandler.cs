using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Question.Command.CreateQuestion
{
    public class CreateQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository) : IRequestHandler<CreateQuestionCommandRequest>
    {
        public async Task Handle(CreateQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new Exception("Exam not found");
            }

            Domain.Entities.Question question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Topic, request.QuestionType, request.Point, request.DisplayOrder, request.Explanation);

            foreach (var optionRequest in request.Options)
            {
                Domain.Entities.QuestionOption option = new();
                option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.DisplayOrder);
                question.AddOption(option);
            }

            await questionRepository.AddQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
