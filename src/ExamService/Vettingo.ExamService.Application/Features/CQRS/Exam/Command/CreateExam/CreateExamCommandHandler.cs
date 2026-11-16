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
                exam.AddMultipleChoiceQuestion(ExamQuestionFactory.Create(exam.Id, questionRequest));
            }

            foreach (var questionRequest in request.TrueFalseQuestions ?? [])
            {
                exam.AddTrueFalseQuestion(ExamQuestionFactory.Create(exam.Id, questionRequest));
            }

            await examRepository.AddAsync(exam);
            await examRepository.SaveChangesAsync();
        }
    }
}


