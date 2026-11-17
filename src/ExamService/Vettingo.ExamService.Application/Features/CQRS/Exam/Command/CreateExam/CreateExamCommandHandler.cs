using FlashMediator;
using Microsoft.Extensions.Logging;
using Vettingo.ExamService.Application.Repository;

namespace Vettingo.ExamService.Application.Features.CQRS.Exam.Command.CreateExam
{
    public class CreateExamCommandHandler(IExamRepository<Domain.Entities.Exam> examRepository, ILogger<CreateExamCommandHandler> logger) : IRequestHandler<CreateExamCommandRequest>
    {
        public async Task Handle(CreateExamCommandRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{HandlerName} isteği işleniyor", nameof(CreateExamCommandHandler));
            Domain.Entities.Exam exam = new();
            exam.CreateExam(request.Title, request.DurationMinutes, request.PassingScore, request.OwnerType, request.CompanyId);

            foreach (var questionRequest in request.MultipleChoiceQuestions ?? [])
            {
                Domain.Entities.MultipleChoiceQuestion question = new();
                question.CreateQuestion(exam.Id, questionRequest.QuestionText, questionRequest.Weight, questionRequest.DisplayOrder);

                foreach (var optionRequest in questionRequest.Options)
                {
                    Domain.Entities.MultipleChoiceOption option = new();
                    option.CreateOption(question.Id, optionRequest.OptionText, optionRequest.IsCorrect, optionRequest.Order);
                    question.AddOption(option);
                }

                exam.AddMultipleChoiceQuestion(question);
            }

            foreach (var questionRequest in request.TrueFalseQuestions ?? [])
            {
                Domain.Entities.TrueFalseQuestion question = new();
                question.CreateQuestion(exam.Id, questionRequest.QuestionText, questionRequest.Weight, questionRequest.DisplayOrder,
                    questionRequest.TrueAnswer ?? "True", questionRequest.FalseAnswer ?? "False");

                foreach (var optionRequest in questionRequest.Options)
                {
                    Domain.Entities.TrueFalseOption option = new();
                    option.CreateOption(question.Id, optionRequest.Type, optionRequest.IsCorrect);
                    question.AddOption(option);
                }

                exam.AddTrueFalseQuestion(question);
            }

            await examRepository.AddAsync(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}


