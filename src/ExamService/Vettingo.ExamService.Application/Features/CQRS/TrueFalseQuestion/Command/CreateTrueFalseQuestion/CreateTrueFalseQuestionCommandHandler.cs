using FlashMediator;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.TrueFalseQuestion.Command.CreateTrueFalseQuestion
{
    public class CreateTrueFalseQuestionCommandHandler(IExamRepository examRepository, IQuestionRepository questionRepository) : IRequestHandler<CreateTrueFalseQuestionCommandRequest>
    {
        public async Task Handle(CreateTrueFalseQuestionCommandRequest request, CancellationToken cancellationToken)
        {
            var exam = await examRepository.GetExamByIdAsync(request.ExamId);

            if (exam is null)
            {
                throw new Exception("Exam not found");
            }

            Domain.Entities.TrueFalseQuestion question = new();
            question.CreateQuestion(request.ExamId, request.QuestionText, request.Topic, request.Point, request.Weight, request.DisplayOrder, request.Explanation, request.CorrectAnswer);

            await questionRepository.AddTrueFalseQuestionAsync(question);
            await questionRepository.SaveChangesAsync();
        }
    }
}
